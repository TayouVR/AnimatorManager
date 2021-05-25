#if VRC_SDK_VRCSDK3
using System;
using System.Collections.Generic;
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
		
		public List<VRCMenuControlProxy> controls = new List<VRCMenuControlProxy>();

		private void InitList() {
			if (menuList is null) {
				for (var i = 0; i < menu.controls.Count; i++) {
					controls.Add(new VRCMenuControlProxy(menu.controls[i]));
				}

				menuList = new ReorderableList(controls, typeof(VRCMenuControlProxy));
				menuList.drawElementCallback += DrawElementCallback;
				menuList.elementHeightCallback += ElementHeightCallback;
				menuList.onAddCallback += ONAddCallback;
				menuList.drawHeaderCallback += DrawHeaderCallback;
			}
		}

		private void DrawHeaderCallback(Rect rect) {
			EditorGUI.LabelField(rect, "Controls");
		}

		private void ONAddCallback(ReorderableList list) {
			VRCExpressionsMenu.Control control = new VRCExpressionsMenu.Control();
			menu.controls.Add(control);
			controls.Add(new VRCMenuControlProxy(control));
		}

		private float ElementHeightCallback(int index) {
			var entity = controls[index];

			float height = EditorGUIUtility.singleLineHeight * 1.25f;
            
			if (entity.isExpanded) {
				height += EditorGUIUtility.singleLineHeight * 1.25f; // Image
				height += EditorGUIUtility.singleLineHeight * 1.25f; // Type
				height += EditorGUIUtility.singleLineHeight * 3.25f; // Type Help box
				height += EditorGUIUtility.singleLineHeight * 1.25f; // Parameter
				height += EditorGUIUtility.singleLineHeight * 1.25f; // Value
				
				height += EditorGUIUtility.singleLineHeight * 1.25f; // Seperator Slider

				switch (entity.Type) {
					case VRCExpressionsMenu.Control.ControlType.Button:
						break;
					case VRCExpressionsMenu.Control.ControlType.Toggle:
						break;
					case VRCExpressionsMenu.Control.ControlType.SubMenu:
						height += EditorGUIUtility.singleLineHeight * 1.25f; // Sub Menu Object Field
						if (entity.subMenu != null && entity.subMenuWrapper == null) {
							entity.subMenuWrapper = new VRCExpressionsMenuWrapper(entity.subMenu, data);
						}

						height += 200;
						if (entity.subMenuWrapper != null) {
							height += entity.subMenuWrapper.MenuList.GetHeight(); // Sub Menu
						}
						break;
					case VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet:
						height += EditorGUIUtility.singleLineHeight * (1.25f) * 2; // Parameters
						height += EditorGUIUtility.singleLineHeight * (1.25f * 3) * 4; // Labels
						break;
					case VRCExpressionsMenu.Control.ControlType.FourAxisPuppet:
						height += EditorGUIUtility.singleLineHeight * (1.25f) * 4; // Parameters
						height += EditorGUIUtility.singleLineHeight * (1.25f * 3) * 4; // Labels
						break;
					case VRCExpressionsMenu.Control.ControlType.RadialPuppet:
						height += EditorGUIUtility.singleLineHeight * (1.25f); // Parameters
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			return height;
		}

		public VRCExpressionsMenuWrapper(VRCExpressionsMenu menu, Data data) : base(data) {
			this.menu = menu;
			InitList();
		}

		private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
			VRCMenuControlProxy entity = controls[index];

			if (entity is null) {
				Debug.Log("[<color=#00DDDD>AnimatorManager</color>] The Entity (" + typeof(VRCMenuControlProxy) + ") was null");
				return;
			}
			
			//Debug.Log("[<color=#00DDDD>AnimatorManager</color>] " + index + " 1: " + entity.isExpanded);
            
			rect.y += 2;
			Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

			entity.isExpanded = EditorGUI.Foldout(_rect, entity.isExpanded, "Name", true);
			// TODO AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHHHHHHHHHHH
			//Debug.Log("[<color=#00DDDD>AnimatorManager</color>] " + index + ": " + entity.isExpanded);
			
			_rect.x += 100;
			_rect.width = rect.width - 100;
			entity.Name = EditorGUI.TextField(_rect, entity.Name);

			
			if (!entity.isExpanded) return;

			rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
			_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
			entity.Icon = (Texture2D)EditorGUI.ObjectField(_rect, "Image", entity.Icon, typeof(Texture2D), true/*, AM_Window.Instance.settingsAsset.useSmallTextureInput ? GUILayout.Height(EditorGUIUtility.singleLineHeight) : GUILayout.Height(20)*/);
			
			rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
			_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
			entity.Type = (VRCExpressionsMenu.Control.ControlType)EditorGUI.EnumPopup(_rect, entity.Type);

			
			rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
			_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight*3);
			//Type Info
			switch (entity.Type)
			{
				case VRCExpressionsMenu.Control.ControlType.Button:
					EditorGUI.HelpBox(_rect, "Click or hold to activate. The button remains active for a minimum 0.2s.\nWhile active the (Parameter) is set to (Value).\nWhen inactive the (Parameter) is reset to zero.", MessageType.Info);
					break;
				case VRCExpressionsMenu.Control.ControlType.Toggle:
					EditorGUI.HelpBox(_rect, "Click to toggle on or off.\nWhen turned on the (Parameter) is set to (Value).\nWhen turned off the (Parameter) is reset to zero.", MessageType.Info);
					break;
				case VRCExpressionsMenu.Control.ControlType.SubMenu:
					EditorGUI.HelpBox(_rect, "Opens another expression menu.\nWhen opened the (Parameter) is set to (Value).\nWhen closed (Parameter) is reset to zero.", MessageType.Info);
					break;
				case VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet:
					EditorGUI.HelpBox(_rect, "Puppet menu that maps the joystick to two parameters (-1 to +1).\nWhen opened the (Parameter) is set to (Value).\nWhen closed (Parameter) is reset to zero.", MessageType.Info);
					break;
				case VRCExpressionsMenu.Control.ControlType.FourAxisPuppet:
					EditorGUI.HelpBox(_rect, "Puppet menu that maps the joystick to four parameters (0 to 1).\nWhen opened the (Parameter) is set to (Value).\nWhen closed (Parameter) is reset to zero.", MessageType.Info);
					break;
				case VRCExpressionsMenu.Control.ControlType.RadialPuppet:
					EditorGUI.HelpBox(_rect, "Puppet menu that sets a value based on joystick rotation. (0 to 1)\nWhen opened the (Parameter) is set to (Value).\nWhen closed (Parameter) is reset to zero.", MessageType.Info);
					break;
			}
			
			rect.y += EditorGUIUtility.singleLineHeight * 3.25f;
			_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
			DrawParameterDropDown(_rect, entity.ParameterProperty, "Parameter");
			
			rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
			_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
			DrawParameterValue(_rect, entity.ParameterProperty, entity.value);
			
			rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
			_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.LabelField(_rect, "", GUI.skin.horizontalSlider);

			//Style
			/*if (controlType == ExpressionsControl.ControlType.Toggle)
			{
				style.intValue = EditorGUILayout.Popup("Visual Style", style.intValue, ToggleStyles);
			}*/

			rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
			//Puppet Parameter Set
			switch (entity.Type)
			{
				case VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet:
					if (entity.SubParameters == null || entity.Labels == null || entity.SubParameters.Length != 2 || entity.Labels.Length != 4) {
						entity.SubParameters = new[]{new VRCMenuControlProxy.VRCParameterProxy(), new VRCMenuControlProxy.VRCParameterProxy()};
						entity.Labels = new VRCExpressionsMenu.Control.Label[4];
					}

			
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawParameterDropDown(_rect, entity.SubParameters[0], "Parameter Horizontal", false);
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
			
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawParameterDropDown(_rect, entity.SubParameters[1], "Parameter Vertical", false);
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

			
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawLabel(_rect, entity.Labels[0], "Label Up");
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
			
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawLabel(_rect, entity.Labels[1], "Label Right");
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
			
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawLabel(_rect, entity.Labels[2], "Label Down");
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
			
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawLabel(_rect, entity.Labels[3], "Label Left");
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
					break;
				case VRCExpressionsMenu.Control.ControlType.FourAxisPuppet:
					if (entity.SubParameters == null || entity.Labels == null || entity.SubParameters.Length != 4 || entity.Labels.Length != 4) {
						entity.SubParameters = new[]{new VRCMenuControlProxy.VRCParameterProxy(), new VRCMenuControlProxy.VRCParameterProxy(), new VRCMenuControlProxy.VRCParameterProxy(), new VRCMenuControlProxy.VRCParameterProxy()};
						entity.Labels = new VRCExpressionsMenu.Control.Label[4];
					}

			
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawParameterDropDown(_rect, entity.SubParameters[0], "Parameter Up", false);
			
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawParameterDropDown(_rect, entity.SubParameters[1], "Parameter Right", false);
			
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawParameterDropDown(_rect, entity.SubParameters[2], "Parameter Down", false);
			
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawParameterDropDown(_rect, entity.SubParameters[3], "Parameter Left", false);

			
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawLabel(_rect, entity.Labels[0], "Label Up");
			
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawLabel(_rect, entity.Labels[1], "Label Right");
			
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawLabel(_rect, entity.Labels[2], "Label Down");
			
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawLabel(_rect, entity.Labels[3], "Label Left");
					break;
				case VRCExpressionsMenu.Control.ControlType.RadialPuppet:
					if (entity.SubParameters == null || entity.SubParameters.Length != 1) {
						entity.SubParameters = new[]{new VRCMenuControlProxy.VRCParameterProxy()};
					}

			
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawParameterDropDown(_rect, entity.SubParameters[0], "Paramater Rotation", false);
					break;
				case VRCExpressionsMenu.Control.ControlType.SubMenu:
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					entity.SubMenu = (VRCExpressionsMenu)EditorGUI.ObjectField(_rect, "Sub Menu", entity.SubMenu, typeof(VRCExpressionsMenu), true);
					if (entity.SubMenu != null) {
						
						rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
						_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
						if (entity.subMenuWrapper == null) {
							entity.subMenuWrapper = new VRCExpressionsMenuWrapper(entity.SubMenu, data);
						}
						entity.subMenuWrapper.MenuList.DoList(_rect);
					}
					break;
				default:
					entity.SubParameters = null;
					entity.Labels = null;
					break;
			}
		}
		
		void DrawLabel(Rect position, VRCExpressionsMenu.Control.Label label, string name)
		{
			EditorGUI.LabelField(position, name);
			EditorGUI.indentLevel += 2;
			position.y += EditorGUIUtility.singleLineHeight * 1.25f;
			label.name = EditorGUI.TextField(new Rect(position), "Name", label.name); 
			position.y += EditorGUIUtility.singleLineHeight * 1.25f;
			label.icon = (Texture2D)EditorGUI.ObjectField(new Rect(position), "Icon", label.icon, typeof(Texture2D), true/*, AM_Window.Instance.settingsAsset.useSmallTextureInput ? GUILayout.Height(EditorGUIUtility.singleLineHeight) : GUILayout.Height(20)*/);
			EditorGUI.indentLevel -= 2;
		}
		
		void DrawParameterDropDown(Rect position, VRCMenuControlProxy.VRCParameterProxy parameter, string name, bool allowBool=true) {
			if (parameter is null) return;
			//Dropdown

			parameter.inputIndex = EditorGUI.Popup(new Rect(position.x, position.y, position.width - 210, position.height), name, parameter.inputIndex, data.GetInputNames());
			{
				if (parameter.inputIndex == 0)
					parameter.name = "";
				else
					parameter.name = data.inputs[parameter.inputIndex].Name;
			}
			//Text field
			parameter.name = EditorGUI.TextField(new Rect(position.width - 180, position.y, 200, position.height), parameter.name);

		}
		
		void DrawParameterValue(Rect position, VRCMenuControlProxy.VRCParameterProxy parameter, float value) {
			//if (parameter is null) return;
			
			var paramDef = data.inputs[parameter.inputIndex];
			if (paramDef != null) {
				switch (paramDef.type) {
					case AnimatorControllerParameterType.Int:
						value = EditorGUI.IntField(position, "Value", Mathf.Clamp((int)value, 0, 255));
						break;
					case AnimatorControllerParameterType.Float:
						value = EditorGUI.FloatField(position, "Value", Mathf.Clamp(value, -1f, 1f));
						break;
					case AnimatorControllerParameterType.Bool:
						value = 1f;
						break;
					case AnimatorControllerParameterType.Trigger:
						Debug.Log("[<color=#00DDDD>AnimatorManager</color>] Trigger Type is not Supported by VRChat");
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			} else {
				EditorGUI.BeginDisabledGroup(true);
				value = EditorGUI.FloatField(position, "Value", value);
				EditorGUI.EndDisabledGroup();
			}
		}
	}
}
#endif