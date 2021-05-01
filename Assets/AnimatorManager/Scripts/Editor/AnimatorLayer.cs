using System;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;

namespace AnimatorManager.Scripts.Editor {
	[Serializable]
	public class AnimatorLayer : Base {
		public string Name {
			get {
				if (stateMachine == null) {
					return "";
				}
				if (String.IsNullOrEmpty(stateMachine.name)) {
					if (data.inputs.Count > 0) {
						return data.inputs[stateMachine.primaryInputIndex].Name;
					} else {
						return "Layer " + data.layers.Count;
					}
				} else {
					return stateMachine.name;
				}
			}
			set => stateMachine.name = value;
		}
		public StateMachine stateMachine;
		
		public AnimatorControllerLayer controllerLayer;

		public AnimatorLayer(Data data) : base(data) {
			stateMachine = new StateMachine(data);
		}
	}
}