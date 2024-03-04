using UnityEngine;


[CreateAssetMenu(menuName = "Tutorial/Conditions/Items Collected", fileName = "TutorialCondition Items Collected")]
public class TutorialConditionItemCollect : TutorialCondition {

	private string origText = "";
	
	public int requiredAmount = 5;
	public ItemData itemToCheckFor;
	public bool showCounter = true;

	private bool fired = false;

	private PlayerItems playerItems;

	public override void Setup() {
		playerItems = GameObject.FindObjectOfType<PlayerItems>();
		origText = tutorialDescription;
	}


	public override void UpdateCondition() {
		if (playerItems) {
			var quantity = playerItems.inventory.GetItemQuantity(itemToCheckFor);
			
			if(showCounter) {
				tutorialDescription = origText + $" ({quantity} / {requiredAmount})";
			}
			
			if (quantity >= requiredAmount && !fired) {
				Complete();
				fired = true;
			} else if (quantity < requiredAmount) {
				fired = false;
				Uncomplete();
			}
		}
	}
}