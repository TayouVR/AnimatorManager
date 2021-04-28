using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;

namespace AnimatorManager.Scripts.Editor {
	public class AnimatorLayer : Base {
		public string name;
		public string Name {
			get {
				if (String.IsNullOrEmpty(name)) {
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
		public StateMachine stateMachine;
		
		public AnimatorControllerLayer controllerLayer;

		public AnimatorLayer(Data data) : base(data) {
			stateMachine = new StateMachine(data);
		}
	}
}