using System;
using UnityEngine;

namespace AnimatorManager.Scripts.Editor {
	[Serializable]
	public class State : Base {
		public int overrideInputIndex;

		private string name;
		public string Name {
			get { return !string.IsNullOrEmpty(name) ? name : linkedInputOption != null ? linkedInputOption.name: ""; }
			set { name = value; }
		}

		public int value;
		
		public InputOption linkedInputOption;
		public bool isNotCollapsed;
		public Motion motion;

		public State(Data data) : base(data) {
		}
	}
}