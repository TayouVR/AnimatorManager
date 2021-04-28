using System;

namespace AnimatorManager.Scripts.Editor {
	[Serializable]
	public class StateProxy : Base {
		public State state;
		public StateMachine stateMachine;
		public StateType type;

		public StateProxy(Data data) : this(StateType.State, data) {
		}

		public StateProxy(StateType type, Data data) : base(data) {
			this.type = type;
			switch (type) {
				case StateType.StateMachine:
					stateMachine = new StateMachine(data);
					break;
				case StateType.State:
				default:
					state = new State(data);
					break;
			}
		}
	}

	public enum StateType {
		State,
		StateMachine
	}
}