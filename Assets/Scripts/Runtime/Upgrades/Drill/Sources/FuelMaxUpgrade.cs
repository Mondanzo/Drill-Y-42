using UnityEngine;

[CreateAssetMenu(menuName = "Drill Upgrades/Fuel Max Upgrade")]
public class FuelMaxUpgrade : DrillUpgrade {
	public float MaxFuelAddition = 500f;

	public FuelMaxUpgrade() {
		UpgradeName = "Fuel Max";
	}

	internal override float ModifyMaxFuel(float retVal) {
		return retVal + MaxFuelAddition;
	}
}
