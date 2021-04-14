using AnimatorManager.Scripts.Editor;
using UnityEngine;

namespace AnimatorManager.Scripts {
	//[CreateAssetMenu(fileName = "FILENAME", menuName = "MENUNAME", order = 0)]
	public class AnimatorManagerSettings : ScriptableObject {
		public AnimatorData lastLoadedAnimatorData;
		public int lastSelectedTab;
	}
}