using UnityEngine;

public class DrillTankFuel : MonoBehaviour {

	private SkinnedMeshRenderer skinnedMeshRenderer;
	private DrillController drillController;
	private float currentFuel = 0;

	private void Start() {
		skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		drillController = GetComponentInParent<DrillController>();
	}

	void Update() {
		var fuelPercentage = drillController.Fuel / drillController.MaxFuel;
		currentFuel = Mathf.Lerp(currentFuel, fuelPercentage * 100, 0.1f);
		skinnedMeshRenderer.SetBlendShapeWeight(0, currentFuel);
	}
}
