using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;
using Object = UnityEngine.Object;

namespace AnimatorManager.Scripts.Editor {
	public class AM_Window : EditorWindow {
        
        private bool vrcSDKfound = false;
        private bool cvrCCKfound = false;
    
        AnimatorData data;

        private AnimatorManagerSettings settingsAsset;
        
        public Object source;

        private int tab = 0;
        private Vector2 tab1scroll;
        private Vector2 tab2scroll;
        private Vector2 tab3scroll;
        private static GUIStyle _greyBox;
        
        private ReorderableList _layerlist;
        private ReorderableList _inputlist;

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

        private void OnEnable() {
            vrcSDKfound = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Contains("VRC_SDK_VRCSDK3");
            cvrCCKfound = AssetDatabase.FindAssets("CCK_CVRAvatarEditor", null).Length > 0;
            
            LoadSettings();

            if (data == null) {
                data = CreateInstance<AnimatorData>();
                SaveSettings();
            }

            _layerlist = new ReorderableList(data.layers, typeof(AnimatorLayer));
            _layerlist.drawElementCallback += DrawLayerElementCallback;
            _layerlist.headerHeight = 0;

            _inputlist = new ReorderableList(data.inputs, typeof(AnimatorInput));
            _inputlist.drawElementCallback += DrawInputElementCallback;
            _inputlist.elementHeightCallback += InputElementHeightCallback;
            _inputlist.headerHeight = 0;
        }

        private float InputElementHeightCallback(int index) {
            var entity = data.inputs[index];

            float height = EditorGUIUtility.singleLineHeight * 1.25f;
            
            if (entity.isNotCollapsed) {
                height += EditorGUIUtility.singleLineHeight * 1.25f;
                height += EditorGUIUtility.singleLineHeight * 1.25f;
                if (entity.type != AnimatorControllerParameterType.Trigger) {
                    height += EditorGUIUtility.singleLineHeight * 1.25f;
                }

                if (entity.type == AnimatorControllerParameterType.Float || entity.type == AnimatorControllerParameterType.Int) {
                    height += entity.optionsListHeight;
                }
                
                height += EditorGUIUtility.singleLineHeight * 0.5f;
            }

            return height;
        }

        private void DrawInputElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
            var entity = data.inputs[index];
            
            rect.y += 2;
            rect.x += 12;
            rect.width -= 12;
            Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            entity.isNotCollapsed = EditorGUI.Foldout(_rect, entity.isNotCollapsed, "Name", true);
            _rect.x += 100;
            _rect.width = rect.width - 100;
            entity.name = EditorGUI.TextField(_rect, entity.name);

            if (entity.isNotCollapsed) {

                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.TextField(_rect, "Parameter Name", entity.parameterName);
                
                
                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                entity.type = (AnimatorControllerParameterType)EditorGUI.EnumPopup(_rect, "Type", entity.type);
                
                
                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                //EditorGUI.Popup(_rect, "Default Value", entity.defaultOptionIndex, entity.GetOptionNames()); // Dropdown for default value
                switch (entity.type) {
                    case AnimatorControllerParameterType.Float:
                        _rect.width -= 100;
                        entity.defaultOptionIndex = EditorGUI.Popup(_rect, "Default Option", entity.defaultOptionIndex, entity.GetOptionNames()); // Dropdown for default value
                        _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                        _rect.width = 90;
                        _rect.x = rect.x + (rect.width - 90);
                        entity.defaultOptionIndex = EditorGUI.IntField(_rect, entity.defaultOptionIndex);
                        break;
                    case AnimatorControllerParameterType.Int:
                        _rect.width -= 100;
                        entity.defaultOptionIndex = EditorGUI.Popup(_rect, "Default Option", entity.defaultOptionIndex, entity.GetOptionNames()); // Dropdown for default value
                        _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                        _rect.width = 90;
                        _rect.x = rect.x + (rect.width - 90);
                        entity.defaultOptionIndex = EditorGUI.IntField(_rect, entity.defaultOptionIndex);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        entity.defaultBool = EditorGUI.Toggle(_rect, "Default Value", entity.defaultBool);
                        break;
                    default:
                        break;
                }
                if (entity.optionsRList != null && (entity.type == AnimatorControllerParameterType.Float || entity.type == AnimatorControllerParameterType.Int)) {
                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    _rect = new Rect(rect.x, rect.y, rect.width, entity.optionsListHeight);
                    entity.optionsRList.DoList(_rect);
                }
            }
        }

        private void DrawLayerElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
            var entity = data.layers[index];
            
            rect.y += 2;
            rect.x += 12;
            rect.width -= 12;
            Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            entity.isNotCollapsed = EditorGUI.Foldout(_rect, entity.isNotCollapsed, "Name", true);
            _rect.x += 100;
            _rect.width = rect.width - 100;
            entity.name = EditorGUI.TextField(_rect, entity.name);
        }

        void OnGUI() {
            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
            centeredStyle.alignment = TextAnchor.UpperCenter;
            centeredStyle.fontSize = 30;
        
        
            GUIStyle connectedButtonLeft = new GUIStyle(GUI.skin.button);
            //connectedButtonLeft.border = new RectOffset(6, 0, 6, 6);
            connectedButtonLeft.margin = new RectOffset(4, 0, 4, 4);
            GUIStyle connectedButtonCenter = new GUIStyle(GUI.skin.button);
            //connectedButtonCenter.border = new RectOffset(0, 0, 1, 1);
            connectedButtonCenter.margin = new RectOffset(0, 0, 4, 4);
            GUIStyle connectedButtonRight = new GUIStyle(GUI.skin.button);
            //connectedButtonRight.border = new RectOffset(0, 1, 1, 1);
            connectedButtonRight.margin = new RectOffset(0, 4, 4, 4);
        
        
            //Header
            GUILayout.Label("Animator Manager", centeredStyle);
        
            //Tab Button Group

            Rect buttonGroup = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Animations", connectedButtonLeft)) {
                tab = 0;
            }
            if (GUILayout.Button("Input", connectedButtonCenter)) {
                tab = 1;
            }
            if (GUILayout.Button("Settings", connectedButtonRight)) {
                tab = 2;
            }
            EditorGUILayout.EndHorizontal();
        
            GUILayout.BeginArea(new Rect(0, 65, position.width, position.height - 65 - EditorGUIUtility.singleLineHeight * 2));
            //pages
            if (tab == 0) {
                tab1scroll = EditorGUILayout.BeginScrollView(tab1scroll);
                _layerlist.DoLayoutList();
                EditorGUILayout.EndScrollView();
            }
        
            if (tab == 1) {
                tab2scroll = EditorGUILayout.BeginScrollView(tab2scroll);
                _inputlist.DoLayoutList();
                /*
                foreach (var input in data.inputs) {
                    input.Draw();
                }*/
                EditorGUILayout.EndScrollView();
            }
        
            if (tab == 2) {
                tab3scroll = EditorGUILayout.BeginScrollView(tab3scroll);
                GUI.enabled = false;
                EditorGUILayout.Toggle("VRChat SDK3 Exists", vrcSDKfound);
                EditorGUILayout.Toggle("ChilloutVR CCK Exists", cvrCCKfound);
                GUI.enabled = true;
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        
            //Footer
            Rect footerArea = new Rect(0, position.height - EditorGUIUtility.singleLineHeight * 2, position.width,
                EditorGUIUtility.singleLineHeight * 2);
            GUILayout.BeginArea(footerArea, _greyBox);
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            source = EditorGUILayout.ObjectField("Animator", source, typeof(AnimatorController), true);
            if (source != null && source != data.referenceAnimator) {
                LoadAnimator(source);
            }
            GUILayout.Button("Reset");
            if (GUILayout.Button("Save")) {
                SaveSettings();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void LoadAnimator(Object o) {
            AnimatorController anim = (AnimatorController) o;
            data.LoadAnimator(anim);
        }
    }
}