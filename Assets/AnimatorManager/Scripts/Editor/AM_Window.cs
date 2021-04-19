using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using Object = UnityEngine.Object;

namespace AnimatorManager.Scripts.Editor {
	public class AM_Window : EditorWindow {
        
        private bool vrcSDKfound = false;
        private bool cvrCCKfound = false;

        private static AM_Window _instance;
        public static AM_Window Instance {
            get {
                if (_instance == null) {
                    _instance = ScriptableObject.CreateInstance<AM_Window>();
                }

                return _instance;
            }
        
        }
        
        public AnimatorController source;

        // settings:
        public AnimatorManagerSettings settingsAsset;
        public AnimatorData data;

        [MenuItem("Tools/Animator Manager")]
        static void Init() {
            // Get existing open window or if none, make a new one:
            AM_Window window = (AM_Window)EditorWindow.GetWindow(typeof(AM_Window));
            window.titleContent = new GUIContent("Animator Manager");
            window.Show();
        }

        private void LoadSettings() {
            if (settingsAsset is null) {
                settingsAsset = Resources.Load<AnimatorManagerSettings>("_AM_Settings");
                Debug.Log("Settings Loaded:\n" + settingsAsset);
            }
        }

        private AnimatorData LookupDataForAnimator(AnimatorController controller) {
             List<AnimatorData> foundDatas = new List<AnimatorData>(Resources.FindObjectsOfTypeAll<AnimatorData>());
            if (foundDatas.Count == 0) return null;
            foreach (var animatorData in foundDatas) {
                if (animatorData.referenceAnimator == controller) {
                    Debug.Log(AssetDatabase.GetAssetPath(animatorData));
                    if (String.IsNullOrEmpty(AssetDatabase.GetAssetPath(animatorData))) {
                        string pathToAsset = AssetDatabase.GenerateUniqueAssetPath(settingsAsset.SavedDataPath + controller.name + ".asset");
                        AssetDatabase.CreateAsset(animatorData, pathToAsset);
                        Debug.Log("SHOULD SAVE ASSET HERE: " + pathToAsset);
                    }
                    return animatorData;
                }
            }

            return null;
        }

        private void OnEnable() {
            vrcSDKfound = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Contains("VRC_SDK_VRCSDK3");
            cvrCCKfound = AssetDatabase.FindAssets("CCK_CVRAvatarEditor", null).Length > 0;
            
            LoadSettings();
        }

        void OnGUI() {
            
            //Header
            GUILayout.Label("Animator Manager", Styles.CenteredStyle);
        
            //Tab Button Group

            settingsAsset.selectedTab = GUILayout.Toolbar(settingsAsset.selectedTab, new[] {"Animations", "Input", "Settings"});
        
            //pages
            GUILayout.BeginArea(new Rect(0, 65, position.width, position.height - 65 - EditorGUIUtility.singleLineHeight * 2));
            if (settingsAsset.animatorData != null) {
                if (settingsAsset.selectedTab == 0) {
                    settingsAsset.animatorData.tab1scroll = EditorGUILayout.BeginScrollView(settingsAsset.animatorData.tab1scroll);
                    settingsAsset.animatorData.layerlist.DoLayoutList();
                    EditorGUILayout.EndScrollView();
                }
            
                if (settingsAsset.selectedTab == 1) {
                    settingsAsset.animatorData.tab2scroll = EditorGUILayout.BeginScrollView(settingsAsset.animatorData.tab2scroll);
                    settingsAsset.animatorData.inputlist.DoLayoutList();
                    EditorGUILayout.EndScrollView();
                }
            }
        
            if (settingsAsset.selectedTab == 2) {
                settingsAsset.tab3scroll = EditorGUILayout.BeginScrollView(settingsAsset.tab3scroll);
                GUI.enabled = false;
                EditorGUILayout.Toggle("VRChat SDK3 Exists", vrcSDKfound);
                EditorGUILayout.Toggle("ChilloutVR CCK Exists", cvrCCKfound);
                GUI.enabled = true;
                EditorGUILayout.LabelField("Import", Styles.HeaderLabel);
                settingsAsset.get1stInputFromCommonCondition = EditorGUILayout.Toggle("Determine Primary Input from most common Transition condition", settingsAsset.get1stInputFromCommonCondition);
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        
            //Footer
            Rect footerArea = new Rect(0, position.height - EditorGUIUtility.singleLineHeight * 2, position.width,
                EditorGUIUtility.singleLineHeight * 2);
            GUILayout.BeginArea(footerArea, Styles.GreyBox);
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            source = (AnimatorController)EditorGUILayout.ObjectField("Animator", source, typeof(AnimatorController), true);
            if (source != null) {
                if (settingsAsset.animatorData == null) {
                    LoadAnimator(source, LookupDataForAnimator(source));
                }
                if (source != settingsAsset.animatorData.referenceAnimator) {
                    LoadAnimator(source, LookupDataForAnimator(source));
                }
            }
            if (GUILayout.Button("Reset to Original State")) {
                if (EditorUtility.DisplayDialog("Reset Animator Data", "Do you really want to reset the Animator Data?\nThe original Animator state will be restored.", "Yes", "No")) {
                    settingsAsset.animatorData.Reset();
                }
            }
            if (GUILayout.Button("Clear")) {
                if (EditorUtility.DisplayDialog("Empty Animator Data", "Do you really want to clear the Animator Data?\n" +
                                                                       "The original Animator will not be touched.\n" +
                                                                       "Only the data for this tool will be deleted.", "Yes", "No")) {
                    settingsAsset.animatorData.Clear();
                }
            }
            if (GUILayout.Button("Save")) {
                settingsAsset.animatorData.Save();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void LoadAnimator(AnimatorController anim, AnimatorData dat = null) {
            if (dat == null) {
                settingsAsset.animatorData = CreateInstance<AnimatorData>();
                string pathToAsset = AssetDatabase.GenerateUniqueAssetPath(settingsAsset.SavedDataPath + anim.name + ".asset");
                AssetDatabase.CreateAsset(settingsAsset.animatorData, pathToAsset);
                Debug.Log("SHOULD SAVE ASSET HERE: " + pathToAsset);
                settingsAsset.animatorData.LoadAnimator(anim);
            } else {
                settingsAsset.animatorData = dat;
            }
        }
    }
}