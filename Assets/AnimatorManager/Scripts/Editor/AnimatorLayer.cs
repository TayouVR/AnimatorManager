﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AnimatorManager.Scripts.Editor {
	public class AnimatorLayer {
		public string name;
		public bool isNotCollapsed = true;
		public int primaryInputIndex;
		public List<AnimatorState> states;
		public ReorderableList statesRList;

		public bool overrideExistingLayers;

		public AnimatorLayer(string name, List<AnimatorState> states) {
			this.name = name;
			this.states = states;

			InitReorderableList();
		}

		private void InitReorderableList() {
			statesRList = new ReorderableList(states, typeof(AnimatorState));
			statesRList.draggable = false;
			statesRList.drawElementCallback += DrawElementCallback;
			statesRList.drawHeaderCallback += DrawHeaderCallback;
		}

		public AnimatorLayer() : this("", new List<AnimatorState>()) {
			
		}

		public void SetStatesFromInputOptions(List<InputOption> inputOptions) {
			states.Clear();
			if (inputOptions != null) {
				foreach (var option in inputOptions) {
					AnimatorState state = new AnimatorState {linkedInputOption = option};
					states.Add(state);
				}
			}

		}

		private void DrawHeaderCallback(Rect rect) {
			
		}

		private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
			var entity = states[index];
			
			rect.y += 2;
			Rect _rect = new Rect(rect);
			_rect.height = EditorGUIUtility.singleLineHeight;
			_rect.width = _rect.width/2 - 10;

			entity.isNotCollapsed = EditorGUI.Foldout(_rect, entity.isNotCollapsed, entity.name);
			
			_rect.x = _rect.x + _rect.width + 20;
			
			entity.animation = (AnimationClip)EditorGUI.ObjectField(_rect, entity.animation, typeof(AnimationClip), true);

			if (entity.isNotCollapsed) {
				
			}
		}

		public List<string> GetStateNames() {
			return states.Select(state => state.name).ToList();
		}
	}
	
	public class AnimatorState {
		public int overrideInputIndex;

		public string name {
			get { return linkedInputOption != null ? linkedInputOption.name: ""; }
		}

		public int value;
		
		public InputOption linkedInputOption;
		public bool isNotCollapsed;
		public AnimationClip animation;
		
	}
}