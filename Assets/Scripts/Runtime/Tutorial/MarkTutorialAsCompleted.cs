using UnityEngine;


public class MarkTutorialAsCompleted : MonoBehaviour {
	public static void CompletedTutorial() {
		PersistentStorage.SetBool("TUTORIAL_COMPLETED", true);
	}
}