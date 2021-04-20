using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;

namespace AnimatorManager.Scripts.Editor {
	public class AnimatorLayer {
		public string name;
		public string Name {
			get {
				if (String.IsNullOrEmpty(name)) {
					if (_associatedData.inputs.Count > 0) {
						return _associatedData.inputs[primaryInputIndex].Name;
					} else {
						return "Layer " + _associatedData.layers.Count;
					}
				} else {
					return name;
				}
			}
			set => name = value;
		}
		public bool isNotCollapsed = true;
		public int primaryInputIndex;
		public List<AnimatorState> states;
		public ReorderableList statesRList;
		public AnimatorControllerLayer associatedLayerInController;

		public bool overrideExistingLayers;
		private AnimatorData _associatedData;

		public AnimatorLayer(string name, List<AnimatorState> states, AnimatorData data) {
			_associatedData = data;
			this.name = name;
			this.states = states;

			InitReorderableList();
		}

		public AnimatorLayer(AnimatorData data) : this("", new List<AnimatorState>(), data) {
			
		}

		private void InitReorderableList() {
			statesRList = new ReorderableList(states, typeof(AnimatorState));
			statesRList.draggable = false;
			statesRList.drawElementCallback += DrawElementCallback;
			statesRList.drawHeaderCallback += DrawHeaderCallback;
			statesRList.onAddDropdownCallback += ONAddDropdownCallback;
			statesRList.elementHeightCallback += ElementHeightCallback;
		}

		private float ElementHeightCallback(int index) {
			var entity = states[index];

			float height = EditorGUIUtility.singleLineHeight * 1.25f;
            
			if (entity.isLayer) {
				if (entity.layer.isNotCollapsed) {
					height += EditorGUIUtility.singleLineHeight * 1.25f;
					height += EditorGUIUtility.singleLineHeight * 1.25f;
					height += entity.layer.statesRList.GetHeight();
                
					height += EditorGUIUtility.singleLineHeight * 0.5f;
				} else {
				}
				if (entity.isNotCollapsed) {
					// if its not a layer, but a state
					height += EditorGUIUtility.singleLineHeight * 1.25f;
					height += EditorGUIUtility.singleLineHeight * 1.25f;
				}
			}

			return height;
		}

		private void ONAddDropdownCallback(Rect buttonrect, ReorderableList list) {
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("State"), false, () => { states.Add(new AnimatorState()); });
			menu.AddItem(new GUIContent("Sub Layer"), false, () => { states.Add(new AnimatorState() { isLayer = true, layer = new AnimatorLayer(_associatedData)}); });
			menu.ShowAsContext();
		}

		public void SetStatesFromInputOptions(List<InputOption> inputOptions) {
			states.Clear();
			if (inputOptions != null) {
				foreach (var option in inputOptions) {
					AnimatorState state = new AnimatorState() {linkedInputOption = option};
					states.Add(state);
				}
			}

		}

		private void DrawHeaderCallback(Rect rect) {
			EditorGUI.LabelField(rect, "States");
		}

		private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
			var entity = states[index];
			
			if (entity.isLayer) {
				_associatedData.DrawLayer(rect, entity.layer);
			} else {
				rect.y += 2;
				Rect _rect = new Rect(rect);
				_rect.height = EditorGUIUtility.singleLineHeight;
				_rect.width = _rect.width/2 - 10;

				entity.isNotCollapsed = EditorGUI.Foldout(_rect, entity.isNotCollapsed, entity.Name);
			
				_rect.x = _rect.x + _rect.width + 20;
			
				entity.animation = (AnimationClip)EditorGUI.ObjectField(_rect, entity.animation, typeof(AnimationClip), true);

				if (entity.isNotCollapsed) {
				}
			}
		}

		public List<string> GetStateNames() {
			return states.Select(state => state.Name).ToList();
		}
	}
	
	public class AnimatorState {
		public int overrideInputIndex;

		private string name;
		public string Name {
			get { return !string.IsNullOrEmpty(name) ? name : linkedInputOption != null ? linkedInputOption.name: ""; }
			set { name = value; }
		}

		public int value;
		
		public InputOption linkedInputOption;
		public bool isNotCollapsed;
		public Motion animation;
		public AnimatorStateMachine animatiorStateMachine;

		public bool isLayer;
		public AnimatorLayer layer;
	}
}