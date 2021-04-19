using System.IO;
using AnimatorManager.Scripts.Editor;
using UnityEditor;
using UnityEngine;

namespace AnimatorManager.Scripts {
	//[CreateAssetMenu(fileName = "FILENAME", menuName = "MENUNAME", order = 0)]
	public class AnimatorManagerSettings : ScriptableObject {
		public AnimatorData animatorData;
		public int selectedTab;
		public Vector2 tab3scroll;
		
		//Input
		public bool get1stInputFromCommonCondition;

		public string SavedDataPath {
			get {
				return Path.Combine(Path.GetDirectoryName(AssetDatabase.GetAssetPath(this)), "SavedData") + Path.PathSeparator;
			}
		}

		public override string ToString() {
			return "Data: " + AssetDatabase.GetAssetPath(animatorData) + "\n" +
			       "Selected Tab: " + selectedTab + "\n" +
			       "###### Inputs ######\n" +
			       "Primary Input from most Common Condition: " + get1stInputFromCommonCondition;
		}
	}
}