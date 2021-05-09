#if VRC_SDK_VRCSDK3
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;

namespace AnimatorManager.Scripts.Editor {
	public class VRCExpressionsMenuWrapper : Base {
		
		private VRCExpressionsMenu menu;
		public ReorderableList menuList;

		public ReorderableList MenuList {
			get {
				InitList();

				return menuList;
			}
		}
		private bool[] collapsedStates = new bool[8];

		private void InitList() {
			if (menuList is null) {
				menuList = new ReorderableList(menu.controls, typeof(VRCExpressionsMenu.Control));
				menuList.drawElementCallback += DrawElementCallback;
			}
		}
		
		public VRCExpressionsMenuWrapper(VRCExpressionsMenu menu, Data data) : base(data) {
			this.menu = menu;
			InitList();
		}

		private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
			VRCExpressionsMenu.Control control = menu.controls[index];
			
			Debug.Log(index + " 1: " + collapsedStates[index]);

			//Foldout
			collapsedStates[index] = EditorGUILayout.Foldout(collapsedStates[index], control.name);
			// TODO AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHHHHHHHHHHH
			Debug.Log(index + ": " + collapsedStates[index]);
			
			
			if (collapsedStates[index]) return;

			//Box
			GUILayout.BeginVertical(GUI.skin.box);
			{
				//Generic params
				EditorGUI.indentLevel += 1;
				{
					control.name = EditorGUILayout.TextField("Name", control.name);
					control.icon = (Texture2D)EditorGUILayout.ObjectField("Image", control.icon, typeof(Texture2D), true/*, AM_Window.Instance.settingsAsset.useSmallTextureInput ? GUILayout.Height(EditorGUIUtility.singleLineHeight) : GUILayout.Height(20)*/);
					control.type = (VRCExpressionsMenu.Control.ControlType)EditorGUILayout.EnumPopup(control.type);

					//Type Info
					switch (control.type)
					{
						case VRCExpressionsMenu.Control.ControlType.Button:
							EditorGUILayout.HelpBox("Click or hold to activate. The button remains active for a minimum 0.2s.\nWhile active the (Parameter) is set to (Value).\nWhen inactive the (Parameter) is reset to zero.", MessageType.Info);
							break;
						case VRCExpressionsMenu.Control.ControlType.Toggle:
							EditorGUILayout.HelpBox("Click to toggle on or off.\nWhen turned on the (Parameter) is set to (Value).\nWhen turned off the (Parameter) is reset to zero.", MessageType.Info);
							break;
						case VRCExpressionsMenu.Control.ControlType.SubMenu:
							EditorGUILayout.HelpBox("Opens another expression menu.\nWhen opened the (Parameter) is set to (Value).\nWhen closed (Parameter) is reset to zero.", MessageType.Info);
							break;
						case VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet:
							EditorGUILayout.HelpBox("Puppet menu that maps the joystick to two parameters (-1 to +1).\nWhen opened the (Parameter) is set to (Value).\nWhen closed (Parameter) is reset to zero.", MessageType.Info);
							break;
						case VRCExpressionsMenu.Control.ControlType.FourAxisPuppet:
							EditorGUILayout.HelpBox("Puppet menu that maps the joystick to four parameters (0 to 1).\nWhen opened the (Parameter) is set to (Value).\nWhen closed (Parameter) is reset to zero.", MessageType.Info);
							break;
						case VRCExpressionsMenu.Control.ControlType.RadialPuppet:
							EditorGUILayout.HelpBox("Puppet menu that sets a value based on joystick rotation. (0 to 1)\nWhen opened the (Parameter) is set to (Value).\nWhen closed (Parameter) is reset to zero.", MessageType.Info);
							break;
					}
					
					DrawParameterDropDown(control.parameter, "Parameter");
					DrawParameterValue(control);
					EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

					//Style
					/*if (controlType == ExpressionsControl.ControlType.Toggle)
					{
						style.intValue = EditorGUILayout.Popup("Visual Style", style.intValue, ToggleStyles);
					}*/

					//Puppet Parameter Set
					switch (control.type)
					{
						case VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet:
							if (control.subParameters == null || control.labels == null || control.subParameters.Length != 2 || control.labels.Length != 4) {
								control.subParameters = new VRCExpressionsMenu.Control.Parameter[2];
								control.labels = new VRCExpressionsMenu.Control.Label[4];
							}

							DrawParameterDropDown(control.subParameters[0], "Parameter Horizontal", false);
							DrawParameterDropDown(control.subParameters[1], "Parameter Vertical", false);

							DrawLabel(control.labels[0], "Label Up");
							DrawLabel(control.labels[1], "Label Right");
							DrawLabel(control.labels[2], "Label Down");
							DrawLabel(control.labels[3], "Label Left");
							break;
						case VRCExpressionsMenu.Control.ControlType.FourAxisPuppet:
							if (control.subParameters == null || control.labels == null || control.subParameters.Length != 4 || control.labels.Length != 4) {
								control.subParameters = new VRCExpressionsMenu.Control.Parameter[4];
								control.labels = new VRCExpressionsMenu.Control.Label[4];
							}

							DrawParameterDropDown(control.subParameters[0], "Parameter Up", false);
							DrawParameterDropDown(control.subParameters[1], "Parameter Right", false);
							DrawParameterDropDown(control.subParameters[2], "Parameter Down", false);
							DrawParameterDropDown(control.subParameters[3], "Parameter Left", false);

							DrawLabel(control.labels[0], "Label Up");
							DrawLabel(control.labels[1], "Label Right");
							DrawLabel(control.labels[2], "Label Down");
							DrawLabel(control.labels[3], "Label Left");
							break;
						case VRCExpressionsMenu.Control.ControlType.RadialPuppet:
							if (control.subParameters == null || control.subParameters.Length != 1) {
								control.subParameters = new VRCExpressionsMenu.Control.Parameter[1];
							}

							DrawParameterDropDown(control.subParameters[0], "Paramater Rotation", false);
							break;
						case VRCExpressionsMenu.Control.ControlType.SubMenu:
							control.subMenu = (VRCExpressionsMenu)EditorGUILayout.ObjectField("Sub Menu", control.subMenu, typeof(VRCExpressionsMenu), true);
							if (control.subMenu != null) {
								new VRCExpressionsMenuWrapper(control.subMenu, data).MenuList.DoLayoutList();
							}
							break;
						default:
							control.subParameters = null;
							control.labels = null;
							break;
					}
				}
				EditorGUI.indentLevel -= 1;
			}
			GUILayout.EndVertical();
		}
		
		void DrawLabel(VRCExpressionsMenu.Control.Label label, string name)
		{
			EditorGUILayout.LabelField(name);
			EditorGUI.indentLevel += 2;
			label.name = EditorGUILayout.TextField("Name", label.name);
			label.icon = (Texture2D)EditorGUILayout.ObjectField("Icon", label.icon, typeof(Texture2D), true/*, AM_Window.Instance.settingsAsset.useSmallTextureInput ? GUILayout.Height(EditorGUIUtility.singleLineHeight) : GUILayout.Height(20)*/);
			EditorGUI.indentLevel -= 2;
		}
		
		void DrawParameterDropDown(VRCExpressionsMenu.Control.Parameter parameter, string name, bool allowBool=true) {
			if (parameter is null) return;
			EditorGUILayout.BeginHorizontal();
			{
				//Dropdown
				int currentIndex = 0;
				for (var i = 0; i < data.inputs.Count; i++) {
					if (data.inputs[i].Name == parameter.name) {
						currentIndex = i;
					}
				}

				currentIndex = EditorGUILayout.Popup(name, currentIndex, data.GetInputNames());
				{
					if (currentIndex == 0)
						parameter.name = "";
					else
						parameter.name = data.inputs[currentIndex].Name;
				}
				//Text field
				parameter.name = EditorGUILayout.TextField(parameter.name, GUILayout.MaxWidth(200));
			}
			EditorGUILayout.EndHorizontal();

		}
		
		void DrawParameterValue(VRCExpressionsMenu.Control control) {
			if (control is null) return;
			if (control.parameter is null) return;
			if (string.IsNullOrEmpty(control.parameter.name)) return;
			
			var paramDef = data.FindInputByName(control.parameter.name);
			if (paramDef != null) {
				if (paramDef.type == AnimatorControllerParameterType.Int)
				{
					control.value = EditorGUILayout.IntField("Value", Mathf.Clamp((int)control.value, 0, 255));
				}
				else if (paramDef.type == AnimatorControllerParameterType.Float)
				{
					control.value = EditorGUILayout.FloatField("Value", Mathf.Clamp(control.value, -1f, 1f));
				}
				else if(paramDef.type == AnimatorControllerParameterType.Bool)
				{
					control.value = 1f;
				}
			} else {
				EditorGUI.BeginDisabledGroup(true);
				control.value = EditorGUILayout.FloatField("Value", control.value);
				EditorGUI.EndDisabledGroup();
			}
		}
	}
}
#endif