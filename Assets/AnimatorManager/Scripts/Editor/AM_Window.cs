using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using Object = UnityEngine.Object;

namespace AnimatorManager.Scripts.Editor {
	public class AM_Window : EditorWindow {
        
        private bool vrcSDKfound = false;
        private bool cvrCCKfound = false;
    
        AnimatorData data;

        private AnimatorManagerSettings settingsAsset;
        
        public AnimatorController source;

        private int tab = 0;
        private Vector2 tab1scroll;
        private Vector2 tab2scroll;
        private Vector2 tab3scroll;
        private static GUIStyle _greyBox;

        [MenuItem("Tools/Animator Manager")]
        static void Init() {
            // Get existing open window or if none, make a new one:
            AM_Window window = (AM_Window)EditorWindow.GetWindow(typeof(AM_Window));
            window.titleContent = new GUIContent("Animator Manager");
            window.Show();
            
            _greyBox = new GUIStyle();
            _greyBox.normal.background = Resources.Load<Texture2D>("gray");
            _greyBox.normal.textColor = Color.white;
            _greyBox.stretchWidth = true;
            _greyBox.margin = new RectOffset(0, 0, 0, 0);
            _greyBox.border = new RectOffset(0, 0, 0, 0);
            _greyBox.alignment = TextAnchor.MiddleLeft;
        }

        public void SaveSettings() {
            if (settingsAsset != null) {
                settingsAsset.lastLoadedAnimatorData = data;
                settingsAsset.lastSelectedTab = tab;
            }
        }

        public void LoadSettings() {
            if (settingsAsset is null) {
                settingsAsset = Resources.Load<AnimatorManagerSettings>("_AM_Settings");
            }

            if (settingsAsset != null) {
                data = settingsAsset.lastLoadedAnimatorData;
                tab = settingsAsset.lastSelectedTab;
            }
            
        }

        public AnimatorData LookupDataForAnimator(AnimatorController controller) {
             List<AnimatorData> foundDatas = new List<AnimatorData>(Resources.FindObjectsOfTypeAll<AnimatorData>());
            if (foundDatas.Count == 0) return null;
            foreach (var animatorData in foundDatas) {
                if (animatorData.referenceAnimator == controller) {
                    return animatorData;
                }
            }

            return null;
        }

        private void OnEnable() {
            vrcSDKfound = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Contains("VRC_SDK_VRCSDK3");
            cvrCCKfound = AssetDatabase.FindAssets("CCK_CVRAvatarEditor", null).Length > 0;
            
            LoadSettings();

            if (data == null) {
                data = CreateInstance<AnimatorData>();
                SaveSettings();
            }
        }

        void OnGUI() {
            
            //Header
            GUILayout.Label("Animator Manager", Styles.CenteredStyle);
        
            //Tab Button Group

            tab = GUILayout.Toolbar(tab, new[] {"Animations", "Input", "Settings"});
        
            GUILayout.BeginArea(new Rect(0, 65, position.width, position.height - 65 - EditorGUIUtility.singleLineHeight * 2));
            //pages
            if (tab == 0) {
                tab1scroll = EditorGUILayout.BeginScrollView(tab1scroll);
                data.layerlist.DoLayoutList();
                EditorGUILayout.EndScrollView();
            }
        
            if (tab == 1) {
                tab2scroll = EditorGUILayout.BeginScrollView(tab2scroll);
                data.inputlist.DoLayoutList();
                EditorGUILayout.EndScrollView();
            }
        
            if (tab == 2) {
                tab3scroll = EditorGUILayout.BeginScrollView(tab3scroll);
                GUI.enabled = false;
                EditorGUILayout.Toggle("VRChat SDK3 Exists", vrcSDKfound);
                EditorGUILayout.Toggle("ChilloutVR CCK Exists", cvrCCKfound);
                GUI.enabled = true;
                EditorGUILayout.LabelField("Import", Styles.HeaderLabel);
                EditorGUILayout.Toggle("Determine Primary Input from most common Transition condition", true);
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        
            //Footer
            Rect footerArea = new Rect(0, position.height - EditorGUIUtility.singleLineHeight * 2, position.width,
                EditorGUIUtility.singleLineHeight * 2);
            GUILayout.BeginArea(footerArea, _greyBox);
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            source = (AnimatorController)EditorGUILayout.ObjectField("Animator", source, typeof(AnimatorController), true);
            if (source != null && source != data.referenceAnimator) {
                LoadAnimator(source, LookupDataForAnimator(source));
            }
            GUILayout.Button("Reset");
            if (GUILayout.Button("Save")) {
                SaveSettings();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void LoadAnimator(AnimatorController anim, AnimatorData dat = null) {
            if (dat != null) {
                data.LoadAnimator(anim);
            } else {
                data = dat;
            }
        }
    }
}