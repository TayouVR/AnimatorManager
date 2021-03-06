using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.ScriptableObjects;
#endif

namespace AnimatorManager.Scripts.Editor {
	public class AM_Window : EditorWindow {
        
        public static bool vrcSDKfound = false;
        public static bool cvrCCKfound = false;

        public static AM_Window Instance {
            get {
                if (_instance == null) {
                    _instance = ScriptableObject.CreateInstance<AM_Window>();
                }

                return _instance;
            }
        
        }
        private static AM_Window _instance;

        public bool vrcStuffCollapsed;
        
        public AnimatorController source;

        // settings:
        public Settings settingsAsset;

        [MenuItem("Tools/Animator Manager")]
        static void Init() {
            // Get existing open window or if none, make a new one:
            AM_Window window = (AM_Window)EditorWindow.GetWindow(typeof(AM_Window));
            window.titleContent = new GUIContent("Animator Manager");
            window.Show();
        }

        private void LoadSettings() {
            if (settingsAsset is null) {
                settingsAsset = Resources.Load<Settings>("_AM_Settings");
                Debug.Log("[<color=#00DDDD>AnimatorManager</color>] Settings Loaded:\n" + settingsAsset);
            }
        }

        private Data LookupDataForAnimator(AnimatorController controller) {
             List<Data> foundDatas = new List<Data>(Resources.FindObjectsOfTypeAll<Data>());
            if (foundDatas.Count == 0) return null;
            foreach (var animatorData in foundDatas) {
                if (animatorData.referenceAnimator == controller) {
                    Debug.Log("[<color=#00DDDD>AnimatorManager</color>] " + AssetDatabase.GetAssetPath(animatorData));
                    if (String.IsNullOrEmpty(AssetDatabase.GetAssetPath(animatorData))) {
                        string pathToAsset = AssetDatabase.GenerateUniqueAssetPath(settingsAsset.SavedDataPath + controller.name + ".asset");
                        AssetDatabase.CreateAsset(animatorData, pathToAsset);
                        Debug.Log("[<color=#00DDDD>AnimatorManager</color>] SHOULD SAVE ASSET HERE: " + pathToAsset);
                    }
                    return animatorData;
                }
            }

            return null;
        }

        private void OnEnable() {
            List<string> scriptingDefineSymbols = GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            vrcSDKfound = scriptingDefineSymbols.Contains("VRC_SDK_VRCSDK3");
            cvrCCKfound = AssetDatabase.FindAssets("CCK_CVRAvatarEditor", null).Length > 0;
            if (cvrCCKfound && !scriptingDefineSymbols.Contains("CVR_CCK_EXISTS")) {
                scriptingDefineSymbols.Add("CVR_CCK_EXISTS");
                SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, scriptingDefineSymbols);
            } else if (!cvrCCKfound && scriptingDefineSymbols.Contains("CVR_CCK_EXISTS")) {
                scriptingDefineSymbols.Remove("CVR_CCK_EXISTS");
                SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, scriptingDefineSymbols);
            }
            LoadSettings();
        }
        
        void OnGUI() {
            //Header
            GUILayout.Label("Animator Manager", Styles.CenteredStyle);
        
            //Tab Button Group
#if VRC_SDK_VRCSDK3
            settingsAsset.selectedTab = GUILayout.Toolbar(settingsAsset.selectedTab, new[] {"Animations", "Input", "Global Settings", "Animator Settings", "VRC Menus"});
#else
            settingsAsset.selectedTab = GUILayout.Toolbar(settingsAsset.selectedTab, new[] {"Animations", "Input", "Global Settings", "Animator Settings"});
#endif
        
            //pages
            GUILayout.BeginArea(new Rect(0, 65, position.width, position.height - 65 - EditorGUIUtility.singleLineHeight * 2));
            if (settingsAsset.data != null) {
                EditorUtility.SetDirty(settingsAsset.data);
                if (settingsAsset.selectedTab == 0) {
                    settingsAsset.data.tab1scroll = EditorGUILayout.BeginScrollView(settingsAsset.data.tab1scroll);
                    settingsAsset.data.layerlist.DoLayoutList();
                    EditorGUILayout.EndScrollView();
                }
            
                if (settingsAsset.selectedTab == 1) {
                    settingsAsset.data.tab2scroll = EditorGUILayout.BeginScrollView(settingsAsset.data.tab2scroll);
#if VRC_SDK_VRCSDK3
                    vrcStuffCollapsed = EditorGUILayout.Foldout(vrcStuffCollapsed, "VRC Stats and Options");
                    if (vrcStuffCollapsed) {
                        //Cost
                        int cost = settingsAsset.data.CalcTotalCost();
                        if(cost <= VRCExpressionParameters.MAX_PARAMETER_COST)
                            EditorGUILayout.HelpBox($"Total Memory: {cost}/{VRCExpressionParameters.MAX_PARAMETER_COST}", MessageType.Info);
                        else
                            EditorGUILayout.HelpBox($"Total Memory: {cost}/{VRCExpressionParameters.MAX_PARAMETER_COST}\nParameters use too much memory.  Remove parameters or use bools which use less memory.", MessageType.Error);

			    
                        //Info
                        EditorGUILayout.HelpBox("Only parameters defined here can be used by expression menus, sync between all playable layers and sync across the network to remote clients.", MessageType.Info);
                        EditorGUILayout.HelpBox("The parameter name and type should match a parameter defined on one or more of your animation controllers.", MessageType.Info);
                        EditorGUILayout.HelpBox("Parameters used by the default animation controllers (Optional)\nVRCEmote, Int\nVRCFaceBlendH, Float\nVRCFaceBlendV, Float", MessageType.Info);

                        //Clear
                        if (GUILayout.Button("Clear Parameters"))
                        {
                            if (EditorUtility.DisplayDialogComplex("Warning", "Are you sure you want to clear all expression parameters?", "Clear", "Cancel", "") == 0)
                            {
                                settingsAsset.data.InitExpressionParameters(false);
                            }
                        }
                        if (GUILayout.Button("Default Parameters"))
                        {
                            if (EditorUtility.DisplayDialogComplex("Warning", "Are you sure you want to reset all expression parameters to default?", "Reset", "Cancel", "") == 0)
                            {
                                settingsAsset.data.InitExpressionParameters(true);
                            }
                        }
                    }
#endif
                    settingsAsset.data.inputlist.DoLayoutList();
                    EditorGUILayout.EndScrollView();
                }
        
                if (settingsAsset.selectedTab == 3) {
                    settingsAsset.data.tab4scroll = EditorGUILayout.BeginScrollView(settingsAsset.data.tab4scroll);
                    settingsAsset.data.DrawSettings();
                    EditorGUILayout.EndScrollView();
                }
#if VRC_SDK_VRCSDK3
                if (settingsAsset.selectedTab == 4) {
                    settingsAsset.data.tab5scroll = EditorGUILayout.BeginScrollView(settingsAsset.data.tab5scroll);
                    settingsAsset.data.mainMenu = (VRCExpressionsMenu)EditorGUILayout.ObjectField("Main Menu", settingsAsset.data.mainMenu, typeof(VRCExpressionsMenu), true);
                    if (settingsAsset.data.mainMenu != null) {
                        settingsAsset.data.DrawVRCMainMenu();
                    }
                    EditorGUILayout.EndScrollView();
                }
#endif
            }
        
            if (settingsAsset.selectedTab == 2) {
                settingsAsset.tab3scroll = EditorGUILayout.BeginScrollView(settingsAsset.tab3scroll);
                
                // detect CVR CCK and VRC SDK
                EditorGUILayout.LabelField("Integrations", Styles.HeaderLabel);
                GUI.enabled = false;
                EditorGUILayout.Toggle("VRChat SDK3 Exists", vrcSDKfound);
                EditorGUILayout.Toggle("ChilloutVR CCK Exists", cvrCCKfound);
                GUI.enabled = true;
                
                // inport
                EditorGUILayout.LabelField("Import", Styles.HeaderLabel);
                settingsAsset.get1stInputFromCommonCondition = EditorGUILayout.Toggle("Determine Primary Input from most common Transition condition", settingsAsset.get1stInputFromCommonCondition);
                
                // misc
                EditorGUILayout.LabelField("Misc", Styles.HeaderLabel);
                settingsAsset.backupCount = EditorGUILayout.IntField("Number of Backups", settingsAsset.backupCount);
                GUI.enabled = false;
                settingsAsset.useSmallTextureInput = EditorGUILayout.Toggle(new GUIContent("Use Small Texture Input", "Currently disabled because it did not work"), settingsAsset.useSmallTextureInput);
                GUI.enabled = true;
                if (GUILayout.Button("Clear ALL Animator Data")) {
                    if (EditorUtility.DisplayDialog("Clear ALL Animator Data", "Do you really want to clear ALL Animator Data?\n" +
                                                                           "The original Animators will not be touched.\n" +
                                                                           "ALL data for them however will be irreversibly deleted.", "Delete it", "Hell Nah")) {
                        DeleteAllDataAssets();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        
            //Footer
            Rect footerArea = new Rect(0, position.height - EditorGUIUtility.singleLineHeight * 2, position.width,
                EditorGUIUtility.singleLineHeight * 2);
            GUILayout.BeginArea(footerArea, Styles.GreyBox);
            GUILayout.Space(5);

            Rect _rect = new Rect(footerArea);
            _rect.y = footerArea.height / 4;
            _rect.x = 5;
            _rect.width = 70;
            EditorGUI.LabelField(_rect,"Animator");
            _rect = new Rect(footerArea);
            _rect.y = footerArea.height / 6;
            _rect.height = footerArea.height * 0.666f;
            _rect.x += 70;
            _rect.width = footerArea.width - 75 - 135 - 85 - 85;
            source = (AnimatorController)EditorGUI.ObjectField(_rect, source, typeof(AnimatorController), true);
            if (source != null) {
                if (settingsAsset.data == null) {
                    LoadAnimator(source, LookupDataForAnimator(source));
                }
                if (source != settingsAsset.data.referenceAnimator) {
                    LoadAnimator(source, LookupDataForAnimator(source));
                }
            }
            _rect.x = footerArea.width - 135 - 85 - 85;
            _rect.width = 130;
            if (GUI.Button(_rect, "Reset Original State")) {
                if (EditorUtility.DisplayDialog("Reset Animator Data", "Do you really want to reset the Animator Data?\nThe original Animator state will be restored.", "Yes", "No")) {
                    settingsAsset.data.Reset();
                }
            }
            _rect.x = footerArea.width - 85 - 85;
            _rect.width = 80;
            if (GUI.Button(_rect, "Clear")) {
                if (EditorUtility.DisplayDialog("Empty Animator Data", "Do you really want to clear the Animator Data?\n" +
                                                                       "The original Animator will not be touched.\n" +
                                                                       "Only the data for this tool will be deleted.", "Yes", "No")) {
                    settingsAsset.data.Clear();
                }
            }
            _rect.x = footerArea.width - 85;
            _rect.width = 80;
            if (GUI.Button(_rect, "Save")) {
                string animatorBackupPath = settingsAsset.SavedDataPath + settingsAsset.data.name + Path.DirectorySeparatorChar;
                if (settingsAsset.backupCount > settingsAsset.data.backupAnimatorControllers.Count) {
                    AssetDatabase.CreateAsset(DuplicateAnimatorController(settingsAsset.data.referenceAnimator), animatorBackupPath + 1);
                    for (var i = 0; i < settingsAsset.data.backupAnimatorControllers.Count; i++) {
                        var path = settingsAsset.data.backupAnimatorControllers[i];
                        AssetDatabase.MoveAsset(path, animatorBackupPath + i+2);
                    }
                }
                settingsAsset.data.Save();
            }
            GUILayout.EndArea();
        }

        private void DeleteAllDataAssets() {
            List<Data> foundDatas = new List<Data>(Resources.FindObjectsOfTypeAll<Data>());
            if (foundDatas.Count == 0) return;
            foreach (var animatorData in foundDatas) {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(animatorData));
            }
        }

        public static AnimatorController DuplicateAnimatorController(AnimatorController input) {
            var output = new AnimatorController();
            output.layers = input.layers;
            output.parameters = input.parameters;
            output.name = input.name;
            //output.animationClips = input.animationClips;
            output.hideFlags = input.hideFlags;
            return output;
        }

        private void LoadAnimator(AnimatorController anim, Data dat = null) {
            if (dat == null) {
                settingsAsset.data = CreateInstance<Data>();
                string pathToAsset = AssetDatabase.GenerateUniqueAssetPath(settingsAsset.SavedDataPath + anim.name + ".asset");
                AssetDatabase.CreateAsset(settingsAsset.data, pathToAsset);
                Debug.Log("[<color=#00DDDD>AnimatorManager</color>] SHOULD SAVE ASSET HERE: " + pathToAsset);
                settingsAsset.data.LoadAnimator(anim);
            } else {
                settingsAsset.data = dat;
                source = dat.referenceAnimator;
            }
        }

        public static List<string> GetScriptingDefineSymbolsForGroup(BuildTargetGroup buildTargetGroup)
        {
            List<string> symbols = new List<string>();
            try
            {
                string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                string[] array = defineSymbols.Split(';');
                if(array != null && array.Length > 0) symbols.AddRange(array);
            }
            catch(System.Exception)
            {
            }
            return symbols;
        }

        public static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup buildTargetGroup, List<string> symbols)
        {
            symbols = CleanList(symbols);
            string symbolsString = string.Empty;
            if(symbols != null && symbols.Count > 0)
            {
                for(int i = 0; i < symbols.Count; i++)
                {
                    if(!string.IsNullOrEmpty(symbols[i]))
                        symbolsString += symbols[i] + ";";
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbolsString);
        }

        /// <summary>
        /// Cleans the given list by removing any whitespaces, any duplicates and any empty entries
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<string> CleanList(List<string> list)
        {
            for(int i = 0; i < list.Count; i++) list[i] = Regex.Replace(list[i], @"\s+", ""); //remove whitespaces
            list = list.Distinct().ToList(); //remove duplicates
            list.RemoveAll(s => string.IsNullOrEmpty(s.Trim())); //remove empty entries
            return list;
        }
    }
}