using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

namespace AnimatorManager.Scripts.Editor {
	[Serializable]
	public class StateMachine : Base {
		public string name;
		public string Name {
			get {
				if (string.IsNullOrEmpty(name)) {
					if (data.inputs.Count > 0) {
						return data.inputs[primaryInputIndex].Name;
					} else {
						return "Layer " + data.layers.Count;
					}
				} else {
					return name;
				}
			}
			set => name = value;
		}
		public bool isNotCollapsed = true;
		public int primaryInputIndex;
		public List<StateProxy> states;
		public ReorderableList statesRList;
		public AnimatorStateMachine controllerStateMachine;
		
		public bool overrideExistingStateMachine;

		public StateMachine(string name, List<StateProxy> states, Data data) : base(data) {
			this.data = data;
			this.name = name;
			this.states = states;

			InitReorderableList();
		}

		public StateMachine(Data data) : this("", new List<StateProxy>(), data) {
			
		}

		private void InitReorderableList() {
			statesRList = new ReorderableList(states, typeof(State));
			statesRList.draggable = false;
			statesRList.drawElementCallback += DrawElementCallback;
			statesRList.drawHeaderCallback += DrawHeaderCallback;
			statesRList.onAddDropdownCallback += ONAddDropdownCallback;
			statesRList.elementHeightCallback += ElementHeightCallback;
		}

		private float ElementHeightCallback(int index) {
			var entity = states[index];

			float height = EditorGUIUtility.singleLineHeight * 1.25f;
            
			switch (entity.type) {
				case StateType.StateMachine: {
					if (entity.stateMachine.isNotCollapsed) {
						height += EditorGUIUtility.singleLineHeight * 1.25f;
						height += EditorGUIUtility.singleLineHeight * 1.25f;
						height += entity.stateMachine.statesRList.GetHeight();
                
						height += EditorGUIUtility.singleLineHeight * 0.5f;
					}

					break;
				}
				case StateType.State: {
					if (entity.state.isNotCollapsed) {
						// if its not a layer, but a state
						height += EditorGUIUtility.singleLineHeight * 1.25f;
						height += EditorGUIUtility.singleLineHeight * 1.25f;
					}

					break;
				}
				default: {
					// if both things are nonexistent something went wrong
					break;
				}
			}

			return height;
		}

		private void ONAddDropdownCallback(Rect buttonrect, ReorderableList list) {
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("State"), false, () => states.Add(new StateProxy(StateType.State, data)));
			menu.AddItem(new GUIContent("Sub Layer"), false, () => states.Add(new StateProxy(StateType.StateMachine, data)));
			menu.ShowAsContext();
		}

		public void SetStatesFromInputOptions(List<InputOption> inputOptions) {
			states.Clear();
			if (inputOptions != null) {
				foreach (var option in inputOptions) {
					StateProxy state = new StateProxy(data);
					state.state.linkedInputOption = option;
					states.Add(state);
				}
			}

		}

		private void DrawHeaderCallback(Rect rect) {
			EditorGUI.LabelField(rect, "States");
		}

		private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
			var entity = states[index];

			switch (entity.type) {
				case StateType.StateMachine:
					data.DrawStateMachine(rect, entity.stateMachine);
					break;
				default: {
					data.DrawState(rect, entity.state);
					break;
				}
			}
		}

		/*public List<string> GetStateNames() {
			return states.Select(state => state.Name).ToList();
		}*/
	}
}