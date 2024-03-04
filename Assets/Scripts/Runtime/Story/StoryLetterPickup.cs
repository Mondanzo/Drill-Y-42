using UnityEngine;

[RequireComponent(typeof(Interaction))]
public class StoryLetterPickup : MonoBehaviour {

	public StoryLetterOrder storyLetters;

	void Start() {
		

		var interaction = GetComponent<Interaction>();
		interaction.Executed.AddListener(PickupStoryPiece);
	}

	private void Update() {
		if (storyLetters.Exists()) {
			gameObject.SetActive(storyLetters.HasLettersLeft());
		} else {
			Destroy(gameObject);
		}
	}

	private void PickupStoryPiece(GameObject pickup) {
		Destroy(gameObject);

		LetterCanvas.ShowLetter(storyLetters.GetNextLetter());
	}
}