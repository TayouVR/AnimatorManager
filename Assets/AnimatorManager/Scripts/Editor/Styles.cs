using UnityEngine;

namespace AnimatorManager.Scripts.Editor {
	public class Styles {

		private static GUIStyle centeredStyle;
		public static GUIStyle CenteredStyle {
			get {
				if (centeredStyle == null) {
					centeredStyle = new GUIStyle(GUI.skin.label);
					centeredStyle.alignment = TextAnchor.UpperCenter;
					centeredStyle.fontSize = 30;
				}
				return centeredStyle;
			}
		}

		private static GUIStyle headerLabel;
		public static GUIStyle HeaderLabel {
			get {
				if (headerLabel == null) {
					headerLabel = new GUIStyle(GUI.skin.label);
					headerLabel.fontStyle = FontStyle.Bold;
				}
				return headerLabel;
			}
		}
		
		private static GUIStyle connectedButtonLeft;
		public static GUIStyle ConnectedButtonLeft {
			get {
				if (connectedButtonLeft == null) {
					connectedButtonLeft = new GUIStyle(GUI.skin.button);
					//connectedButtonLeft.border = new RectOffset(6, 0, 6, 6);
					connectedButtonLeft.margin = new RectOffset(4, 0, 4, 4);
				}
				return connectedButtonLeft;
			}
		}
		
		private static GUIStyle connectedButtonCenter;
		public static GUIStyle ConnectedButtonCenter {
			get {
				if (connectedButtonCenter == null) {
					connectedButtonCenter = new GUIStyle(GUI.skin.button);
					//connectedButtonCenter.border = new RectOffset(0, 0, 1, 1);
					connectedButtonCenter.margin = new RectOffset(0, 0, 4, 4);
				}
				return connectedButtonCenter;
			}
		}
		private static GUIStyle connectedButtonRight;
		public static GUIStyle ConnectedButtonRight {
			get {
				if (connectedButtonRight == null) {
					connectedButtonRight = new GUIStyle(GUI.skin.button);
					//connectedButtonRight.border = new RectOffset(0, 1, 1, 1);
					connectedButtonRight.margin = new RectOffset(0, 4, 4, 4);
				}
				return connectedButtonRight;
			}
		}
	}
}