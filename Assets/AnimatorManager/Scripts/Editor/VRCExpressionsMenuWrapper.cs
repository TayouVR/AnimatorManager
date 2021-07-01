#if VRC_SDK_VRCSDK3
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;
using ExpressionsMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;
using ExpressionControl = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control;
using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;

namespace AnimatorManager.Scripts.Editor {
	public class VRCExpressionsMenuWrapper : Base {
		
		private VRCExpressionsMenu menu;
		public ReorderableList list;
		private VRCExpressionsMenuWrapper[] subMenuWrappers = new VRCExpressionsMenuWrapper[8];

		public ReorderableList MenuList {
			get {
				InitList();

				return list;
			}
		}
		

		private SerializedProperty controls;
		private SerializedObject serializedObject;

		private void InitList() {
			if (list is null) {

				list = new ReorderableList(serializedObject, controls);
				list.drawElementCallback += DrawElementCallback;
				list.elementHeightCallback += ElementHeightCallback;
				list.onAddCallback += ONAddCallback;
				list.onRemoveCallback = ONRemoveCallback;
				list.drawHeaderCallback += DrawHeaderCallback;
		
				if (controls.arraySize >= ExpressionsMenu.MAX_CONTROLS -1) {
					list.displayAdd = false;
				}
			}
		}
		
		private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
			var control = controls.GetArrayElementAtIndex(index);
			DrawControl(rect, controls, control as SerializedProperty, index);
		}

		private void DrawHeaderCallback(Rect rect) {
			EditorGUI.LabelField(rect, "Controls");
		}

		private float ElementHeightCallback(int index) {
			var control = controls.GetArrayElementAtIndex(index);
			
			var type = control.FindPropertyRelative("type");
			var subMenu = control.FindPropertyRelative("subMenu");

			float height = EditorGUIUtility.singleLineHeight * 1.25f;
	            
			if (control.isExpanded) {
				height += EditorGUIUtility.singleLineHeight * 1.25f; // Image
				height += EditorGUIUtility.singleLineHeight * 1.25f; // Type
				height += EditorGUIUtility.singleLineHeight * 3.25f; // Type Help box
				height += EditorGUIUtility.singleLineHeight * 1.25f; // Parameter
				height += EditorGUIUtility.singleLineHeight * 1.25f; // Value
					
				height += EditorGUIUtility.singleLineHeight * 1.25f; // Seperator Slider
				height += EditorGUIUtility.singleLineHeight * 1.25f; // ??

				var controlType = (ExpressionControl.ControlType)type.intValue;
				switch (controlType) {
					case VRCExpressionsMenu.Control.ControlType.Button:
						break;
					case VRCExpressionsMenu.Control.ControlType.Toggle:
						break;
					case VRCExpressionsMenu.Control.ControlType.SubMenu:
						height += EditorGUIUtility.singleLineHeight * 1.25f; // Sub Menu Object Field

						//height += 200;
						/*if ((object)subMenu.serializedObject.targetObject != null && serializedObject != subMenu.serializedObject) {
							Debug.Log("aaaaaa");
							return 1;
							if (subMenuWrappers[index] == null) {
								subMenuWrappers[index] = new VRCExpressionsMenuWrapper(subMenu.serializedObject, data);
							}
							height += subMenuWrappers[index].list.GetHeight(); // Sub Menu
						}*/
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

		private void ONRemoveCallback(ReorderableList reorderableList) {
			controls.DeleteArrayElementAtIndex(reorderableList.index);
			if (controls.arraySize < ExpressionsMenu.MAX_CONTROLS && controls.arraySize > 0) {
				list.displayAdd = true;
			}
		}

		private void ONAddCallback(ReorderableList reorderableList) {
			var menu = serializedObject.targetObject as ExpressionsMenu;

			var control = new ExpressionControl();
			control.name = "New Control";
			menu.controls.Add(control);
			if (controls.arraySize >= ExpressionsMenu.MAX_CONTROLS -1) {
				list.displayAdd = false;
			}
		}

		public VRCExpressionsMenuWrapper(SerializedObject menu, Data data) : base(data) {
			serializedObject = menu;
			controls = serializedObject.FindProperty("controls");
			/*for (int i = 0; i < controls.arraySize; i++) {
				subMenuWrappers[i] = new VRCExpressionsMenuWrapper(controls.GetArrayElementAtIndex(i).FindPropertyRelative("subMenu").serializedObject, data);
			}*/
			InitList();
		}

		void DrawControl(Rect rect, SerializedProperty control, SerializedProperty entity, int index)
		{
			var name = entity.FindPropertyRelative("name");
			var icon = entity.FindPropertyRelative("icon");
			var type = entity.FindPropertyRelative("type");
			var parameter = entity.FindPropertyRelative("parameter");
			var value = entity.FindPropertyRelative("value");
			var subMenu = entity.FindPropertyRelative("subMenu");

			var subParameters = entity.FindPropertyRelative("subParameters");
			var labels = entity.FindPropertyRelative("labels");

			//Foldout
			EditorGUI.BeginChangeCheck();
	            
			rect.y += 2;
			Rect _rect = new Rect(rect.x + 10, rect.y, rect.width - 10, EditorGUIUtility.singleLineHeight);

			entity.isExpanded = EditorGUI.Foldout(_rect, entity.isExpanded, name.stringValue, true);
			
			if (!entity.isExpanded)
				return;

			{

				//Generic params
				{

					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					EditorGUI.PropertyField(_rect, name);

					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					EditorGUI.PropertyField(_rect, icon);

					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					EditorGUI.PropertyField(_rect, type);
					
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight*3);
					//Type Info
					var controlType = (ExpressionControl.ControlType)type.intValue;
					switch (controlType)
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
					DrawParameterDropDown(_rect, parameter, "Parameter");
				
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					DrawParameterValue(_rect, parameter, value);
				
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
					_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
					EditorGUI.LabelField(_rect, "", GUI.skin.horizontalSlider);
					rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

					//Style
					/*if (controlType == ExpressionsControl.ControlType.Toggle)
					{
						style.intValue = EditorGUILayout.Popup("Visual Style", style.intValue, ToggleStyles);
					}*/

					//Puppet Parameter Set
					switch (controlType)
					{
						case ExpressionControl.ControlType.TwoAxisPuppet:
							subParameters.arraySize = 2;
							labels.arraySize = 4;

				
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawParameterDropDown(_rect, subParameters.GetArrayElementAtIndex(0), "Parameter Horizontal", false);
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
				
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawParameterDropDown(_rect, subParameters.GetArrayElementAtIndex(1), "Parameter Vertical", false);
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

				
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawLabel(_rect, labels.GetArrayElementAtIndex(0), "Label Up");
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
				
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawLabel(_rect, labels.GetArrayElementAtIndex(1), "Label Right");
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
				
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawLabel(_rect, labels.GetArrayElementAtIndex(2), "Label Down");
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
				
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawLabel(_rect, labels.GetArrayElementAtIndex(3), "Label Left");
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
							break;
						case ExpressionControl.ControlType.FourAxisPuppet:
							subParameters.arraySize = 4;
							labels.arraySize = 4;

							rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawParameterDropDown(_rect, subParameters.GetArrayElementAtIndex(0), "Parameter Up", false);
				
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawParameterDropDown(_rect, subParameters.GetArrayElementAtIndex(1), "Parameter Right", false);
				
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawParameterDropDown(_rect, subParameters.GetArrayElementAtIndex(2), "Parameter Down", false);
				
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawParameterDropDown(_rect, subParameters.GetArrayElementAtIndex(3), "Parameter Left", false);

				
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawLabel(_rect, labels.GetArrayElementAtIndex(0), "Label Up");
				
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawLabel(_rect, labels.GetArrayElementAtIndex(1), "Label Right");
				
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawLabel(_rect, labels.GetArrayElementAtIndex(2), "Label Down");
				
							rect.y += EditorGUIUtility.singleLineHeight * 1.25f * 3;
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawLabel(_rect, labels.GetArrayElementAtIndex(3), "Label Left");
							break;
						case ExpressionControl.ControlType.RadialPuppet:
							subParameters.arraySize = 1;
							labels.arraySize = 0;

							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							DrawParameterDropDown(_rect, subParameters.GetArrayElementAtIndex(0), "Paramater Rotation", false);
							break;
						case VRCExpressionsMenu.Control.ControlType.SubMenu:
							_rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
							EditorGUI.PropertyField(_rect, subMenu);
							Debug.Log(subMenu.serializedObject.targetObject.name);
							break;
						default:
							subParameters.arraySize = 0;
							labels.arraySize = 0;
							break;
					}
				}
			}
		}
		
		void DrawLabel(Rect rect, SerializedProperty subControl, string name)
		{
			var nameProp = subControl.FindPropertyRelative("name");
			var icon = subControl.FindPropertyRelative("icon");
			
			EditorGUI.LabelField(rect, name);
			EditorGUI.indentLevel += 2;
			rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
			EditorGUI.PropertyField(new Rect(rect), nameProp); 
			rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
			EditorGUI.PropertyField(new Rect(rect), icon);
			EditorGUI.indentLevel -= 2;
		}
		
		void DrawParameterDropDown(Rect rect, SerializedProperty parameter, string name, bool allowBool=true) {
			var parameterName = parameter.FindPropertyRelative("name");
			string value = parameterName.stringValue;

			bool parameterFound = false;
			EditorGUILayout.BeginHorizontal();
			{
				//Dropdown
				int currentIndex;
				if (string.IsNullOrEmpty(value))
				{
					currentIndex = -1;
					parameterFound = true;
				}
				else
				{
					currentIndex = -2;
					for (int i = 0; i < data.inputs.Count; i++)
					{
						var item = data.inputs[i].parameterName;
						if (item == value)
						{
							parameterFound = true;
							currentIndex = i;
							break;
						}
					}
				}

				//Dropdown
				EditorGUI.BeginChangeCheck();
				currentIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width - 220, rect.height), name, currentIndex + 1, data.GetInputNames());
				if (EditorGUI.EndChangeCheck())
				{
					if (currentIndex == 0)
						parameterName.stringValue = "";
					else
						parameterName.stringValue = data.inputs[currentIndex].parameterName;
				}

				//Text field
				parameterName.stringValue = EditorGUI.TextField(new Rect(rect.width - 180, rect.y, 200, rect.height), parameterName.stringValue);
			}
			EditorGUILayout.EndHorizontal();

		}
		
		void DrawParameterValue(Rect position, SerializedProperty parameter, SerializedProperty value) {
			//if (parameter is null) return;
			
			string paramName = parameter.FindPropertyRelative("name").stringValue;
			Input paramDef = null;
			var names = data.GetInputNames();
			for (var i = 0; i < names.Length; i++) {
				if (names[i] == paramName) {
					paramDef = data.inputs[i];
				}
			}

			if (paramDef != null) {
				switch (paramDef.type) {
					case AnimatorControllerParameterType.Int:
						value.floatValue = EditorGUI.IntField(position, "Value", Mathf.Clamp((int)value.floatValue, 0, 255));
						break;
					case AnimatorControllerParameterType.Float:
						value.floatValue = EditorGUI.FloatField(position, "Value", Mathf.Clamp(value.floatValue, -1f, 1f));
						break;
					case AnimatorControllerParameterType.Bool:
						value.floatValue = 1f;
						break;
					case AnimatorControllerParameterType.Trigger:
						Debug.Log("[<color=#00DDDD>AnimatorManager</color>] Trigger Type is not Supported by VRChat");
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			} else {
				EditorGUI.BeginDisabledGroup(true);
				value.floatValue = EditorGUI.FloatField(position, "Value", value.floatValue);
				EditorGUI.EndDisabledGroup();
			}
		}
	}
}
#endif