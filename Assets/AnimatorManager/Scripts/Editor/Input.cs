using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;

namespace AnimatorManager.Scripts.Editor {
	[Serializable]
	public class Input : Base {
		private string name;
		public string Name {
			get {
				if (String.IsNullOrEmpty(name)) {
					if (String.IsNullOrEmpty(parameterName)) {
						return "Input " + data.inputs.Count;
					} else {
						return parameterName;
					}
				} else {
					return name;
				}
			}
			set { name = value; }
		}

		public bool isNotCollapsed = true;
		public string parameterName;

		public ReorderableList OptionsRList {
			get { 
				if (optionsRList == null) {
					optionsRList = new ReorderableList(options, typeof(InputOption));
					optionsRList.drawElementCallback += DrawElementCallback;
					optionsRList.drawHeaderCallback += DrawHeaderCallback;
					optionsRList.onAddCallback += OnAddCallback;
				}
				return optionsRList; }
			set { optionsRList = value; }
		}

		public AnimatorControllerParameterType previousType = AnimatorControllerParameterType.Bool;
		public AnimatorControllerParameterType type = AnimatorControllerParameterType.Bool;
		public List<InputOption> options = new List<InputOption>();
		private ReorderableList optionsRList;
		public float defaultFloat;
		public int defaultInt;
		public bool defaultBool;
		public int defaultOptionIndex;
		
		// VRC
		public bool saved = true;

		public Input(Data data) : base(data) {
		}

		private void OnAddCallback(ReorderableList list) {
			var temp = new InputOption();
			switch (type) {
				case AnimatorControllerParameterType.Float: {
					float lastValue = 0;
					float secondLastValue = -1;
					if (list.list.Count >= 2) {
						InputOption secondLast = (InputOption) list.list[list.list.Count - 2];
						secondLastValue = secondLast?.floatValue ?? 1;
					}
					if (list.list.Count >= 1) {
						InputOption last = (InputOption) list.list[list.list.Count - 1];
						lastValue = last?.floatValue ?? 0;
					} else {
						secondLastValue = 0;
					}
					float distanceBetweenLastTwoValues = lastValue - secondLastValue;
					temp.floatValue = lastValue + distanceBetweenLastTwoValues;
					break;
				}
				case AnimatorControllerParameterType.Int: {
					int lastValue = 0;
					int secondLastValue = -1;
					if (list.list.Count >= 2) {
						InputOption secondLast = (InputOption) list.list[list.list.Count - 2];
						secondLastValue = secondLast?.intValue ?? 1;
					}
					if (list.list.Count >= 1) {
						InputOption last = (InputOption) list.list[list.list.Count - 1];
						lastValue = last?.intValue ?? 0;
					} else {
						secondLastValue = 0;
					}
					int distanceBetweenLastTwoValues = lastValue - secondLastValue;
					temp.intValue = lastValue + distanceBetweenLastTwoValues;
					break;
				}
				case AnimatorControllerParameterType.Bool:
					break;
				case AnimatorControllerParameterType.Trigger:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			list.list.Add(temp);
		}

		private void DrawHeaderCallback(Rect rect) {
			rect.y += 2;
			rect.x += 12;
			rect.width += 30;
			Rect _rect = new Rect(rect.x, rect.y, rect.width - 100, EditorGUIUtility.singleLineHeight);
			
			EditorGUI.LabelField(_rect, "Name");
			
			_rect = new Rect(rect.width -90, rect.y, 90, EditorGUIUtility.singleLineHeight);
			
			EditorGUI.LabelField(_rect, "Value");
		}

		private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused) {

			rect.y += 2;
			Rect _rect = new Rect(rect);
			_rect.height = EditorGUIUtility.singleLineHeight;
			_rect.width += -150;
			
			options[index].name = EditorGUI.TextField(_rect, options[index].name);
			
			_rect.x = _rect.x + _rect.width + 50;
			_rect.width = 100;
			
			
			switch (type) {
				case AnimatorControllerParameterType.Float:
					options[index].floatValue = EditorGUI.FloatField(_rect, options[index].floatValue);
					break;
				case AnimatorControllerParameterType.Int:
					options[index].intValue = EditorGUI.IntField(_rect, options[index].intValue);
					break;
				default:
					Debug.Log("Wrong Type");
					break;
			}
		}

		public string[] GetOptionNames() {

			List<string> names = new List<string>();

			foreach (var option in options) {
				if (string.IsNullOrEmpty(option.name)) {
					switch (type) {
						case AnimatorControllerParameterType.Float:
							names.Add(option.floatValue.ToString());
							break;
						case AnimatorControllerParameterType.Int:
							names.Add(option.intValue.ToString());
							break;
						default:
							Debug.Log("Wrong Type");
							break;
					}
				}else {
					names.Add(option.name);
				}
			}

			return names.ToArray();
		}
	}
}