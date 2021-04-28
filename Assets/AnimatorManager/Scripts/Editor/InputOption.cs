namespace AnimatorManager.Scripts.Editor {
	public class InputOption {
		public string name;
		public int intValue;
		public float floatValue;

		public InputOption() {
		}

		public InputOption(string name) {
			this.name = name;
		}

		public InputOption(string name, int intValue, float floatValue) {
			this.name = name;
			this.intValue = intValue;
			this.floatValue = floatValue;
		}
	}
}