using System;

namespace AnimatorManager.Scripts.Editor {
	[Serializable]
	public class StateProxy : Base {
		public State state;
		public StateMachine stateMachine;
		public StateType type;

		public StateProxy(Data data, StateMachine parentMachine) : this(StateType.State, data, parentMachine) {
		}

		public StateProxy(StateType type, Data data, StateMachine parentMachine) : base(data) {
			this.type = type;
			switch (type) {
				case StateType.StateMachine:
					stateMachine = new StateMachine(data);
					break;
				case StateType.State:
				default:
					state = new State(data, parentMachine);
					break;
			}
		}
	}

	public enum StateType {
		State,
		StateMachine
	}
}