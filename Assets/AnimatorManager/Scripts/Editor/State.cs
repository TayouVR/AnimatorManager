using System;
using UnityEngine;

namespace AnimatorManager.Scripts.Editor {
	[Serializable]
	public class State : Base {
		public int overrideInputIndex;

		public string name;
		public string Name {
			get {
				//Debug.Log("[<color=#00DDDD>AnimatorManager</color>] Input: " + stateMachinePrimaryInputIndex + "\nInput Option: " + linkedInputOptionIndex);
				//Debug.Log("[<color=#00DDDD>AnimatorManager</color>] Input Count: " + data.inputs.Count + "\nInput Option Count: " + data.inputs[stateMachinePrimaryInputIndex].options.Count);
				if (linkedInputOptionIndex > -1) {
					if (data.inputs.Count > stateMachinePrimaryInputIndex) {
						if (data.inputs[stateMachinePrimaryInputIndex].options.Count > linkedInputOptionIndex) {
							return data.inputs[stateMachinePrimaryInputIndex].options[linkedInputOptionIndex].name;
						}
						Debug.Log("[<color=#00DDDD>AnimatorManager</color>] Input Option out of range for State");
						return "State " + stateNumber;
					}
					Debug.Log("[<color=#00DDDD>AnimatorManager</color>] Input out of range for State");
					return "State " + stateNumber;
				} else if (string.IsNullOrEmpty(name)) {
					return "State " + stateNumber;
				} else {
					return name;
				}
			}
			set { name = value; }
		}

		public int value;
		
		public int linkedInputOptionIndex = -1;
		private int stateMachinePrimaryInputIndex;
		public bool isNotCollapsed;
		public Motion motion;
		private int stateNumber;

		public State(Data data, StateMachine machine) : base(data) {
			stateNumber = machine.states.Count;
			stateMachinePrimaryInputIndex = machine.primaryInputIndex;
		}
	}
}