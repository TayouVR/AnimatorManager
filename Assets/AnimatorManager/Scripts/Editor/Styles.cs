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
		
		public static GUIStyle EndNode {
			get {
				if (_endNode == null) {
					_endNode = new GUIStyle();
					_endNode.normal.background = Resources.Load<Texture2D>("red");
					_endNode.focused.background = Resources.Load<Texture2D>("red");
					_endNode.normal.textColor = Color.white;
					_endNode.alignment = TextAnchor.MiddleCenter;
					_endNode.border = new RectOffset(16, 16, 16, 16);
				}
				return _endNode;
			}
		}
		private static GUIStyle _endNode;
		
		public static GUIStyle StartNode {
			get {
				if (_startNode == null) {
					_startNode = new GUIStyle();
					_startNode.normal.background = Resources.Load<Texture2D>("green");
					_startNode.focused.background = Resources.Load<Texture2D>("green");
					_startNode.normal.textColor = Color.white;
					_startNode.alignment = TextAnchor.MiddleCenter;
					_startNode.border = new RectOffset(16, 16, 16, 16);
				}
				return _startNode;
			}
		}
		private static GUIStyle _startNode;
		
		public static GUIStyle AnyStateNode {
			get {
				if (_anyStateNode == null) {
					_anyStateNode = new GUIStyle();
					_anyStateNode.normal.background = Resources.Load<Texture2D>("anyStateNode");
					_anyStateNode.onFocused.background = Resources.Load<Texture2D>("anyStateNode");
					_anyStateNode.focused.background = Resources.Load<Texture2D>("anyStateNode");
					_anyStateNode.normal.textColor = Color.white;
					_anyStateNode.alignment = TextAnchor.MiddleCenter;
					_anyStateNode.border = new RectOffset(16, 16, 16, 16);
				}
				return _anyStateNode;
			}
		}
		private static GUIStyle _anyStateNode;
		
		public static GUIStyle BlankStateNode {
			get {
				if (_blankStateNode == null) {
					_blankStateNode = new GUIStyle();
					_blankStateNode.normal.background = Resources.Load<Texture2D>("");
					_blankStateNode.onFocused.background = Resources.Load<Texture2D>("");
					_blankStateNode.focused.background = Resources.Load<Texture2D>("");
					_blankStateNode.normal.textColor = Color.white;
					_blankStateNode.alignment = TextAnchor.MiddleCenter;
					_blankStateNode.border = new RectOffset(16, 16, 16, 16);
				}
				return _blankStateNode;
			}
		}
		private static GUIStyle _blankStateNode;
	}
}