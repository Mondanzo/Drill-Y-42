using UnityEngine;


[CreateAssetMenu(menuName = "Tutorial/Conditions/Stimpack Doses", fileName = "TutorialStep Stimpack Doses Charged")]
public class TutorialConditionStimpackDoses : TutorialCondition {

	public int requiredAmount = 1;
	public bool showCounter = true;

	private Stimpack playerStimpack;

	private bool fired = false;
	private string origText;
	 
	public override void Setup() {
		playerStimpack = FindObjectOfType<Stimpack>();
		origText = tutorialDescription;
	}

	public override void UpdateCondition() { 
		if (cantBeUncompleted && isCompleted) return;
		
		if(showCounter) {
			tutorialDescription = origText + $" ({playerStimpack.doses} / {requiredAmount})";
		}
		
		if (playerStimpack.doses >= requiredAmount && !fired) {
			fired = true;
			Complete();
		} else if (playerStimpack.doses < requiredAmount) {
			fired = false;
			Uncomplete();
		}
	}
}