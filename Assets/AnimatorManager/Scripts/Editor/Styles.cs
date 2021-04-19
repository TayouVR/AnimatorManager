using UnityEngine;

namespace AnimatorManager.Scripts.Editor {
	public class Styles {

		public static GUIStyle CenteredStyle {
			get {
				if (_centeredStyle == null) {
					_centeredStyle = new GUIStyle(GUI.skin.label);
					_centeredStyle.alignment = TextAnchor.UpperCenter;
					_centeredStyle.fontSize = 30;
				}
				return _centeredStyle;
			}
		}
		private static GUIStyle _centeredStyle;

		public static GUIStyle HeaderLabel {
			get {
				if (_headerLabel == null) {
					_headerLabel = new GUIStyle(GUI.skin.label);
					_headerLabel.fontStyle = FontStyle.Bold;
				}
				return _headerLabel;
			}
		}
		private static GUIStyle _headerLabel;
		
		public static GUIStyle GreyBox {
			get {
				if (_greyBox == null) {
					_greyBox = new GUIStyle();
					_greyBox.normal.background = Resources.Load<Texture2D>("gray");
					_greyBox.normal.textColor = Color.white;
					_greyBox.stretchWidth = true;
					_greyBox.margin = new RectOffset(0, 0, 0, 0);
					_greyBox.border = new RectOffset(0, 0, 0, 0);
					_greyBox.alignment = TextAnchor.MiddleLeft;
				}
				return _greyBox;
			}
		}
		private static GUIStyle _greyBox;
	}
}