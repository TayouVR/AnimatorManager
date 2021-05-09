using System.IO;
using UnityEditor;
using UnityEngine;

namespace AnimatorManager.Scripts.Editor {
	//[CreateAssetMenu(fileName = "FILENAME", menuName = "MENUNAME", order = 0)]
	public class Settings : ScriptableObject {
		public Data data;
		public int selectedTab;
		public Vector2 tab3scroll;
		
		//Input
		public bool get1stInputFromCommonCondition = true;

		//Misc
		public int backupCount;
		public bool useSmallTextureInput;
		
		public string SavedDataPath {
			get {
				return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(AssetDatabase.GetAssetPath(this))), "SavedData") + Path.DirectorySeparatorChar;
			}
		}
		public string AnimationsPath {
			get {
				return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(AssetDatabase.GetAssetPath(this))), "Animations") + Path.DirectorySeparatorChar;
			}
		}

		public override string ToString() {
			return "Data: " + AssetDatabase.GetAssetPath(data) + "\n" +
			       "Selected Tab: " + selectedTab + "\n" +
			       "###### Inputs ######\n" +
			       "Primary Input from most Common Condition: " + get1stInputFromCommonCondition + "\n";
		}
	}
}