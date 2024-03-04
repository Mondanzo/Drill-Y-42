using UnityEngine;
using UnityEngine.Events;

public class OnDrillUpdateAdded : MonoBehaviour {

	private DrillData drillData;

	[SerializeField]
	private DrillUpgrade drillUpgradeToReactTo;

	[SerializeField]
	private UnityEvent upgradeWasAdded;

	private void Start() {
		drillData = GetComponentInParent<DrillData>();
		drillData.UpgradeAdded.AddListener(dUpgradeAdded);
	}

	private void dUpgradeAdded(DrillUpgrade arg0) {
		if (GameObject.ReferenceEquals(arg0, drillUpgradeToReactTo)) {
			upgradeWasAdded.Invoke();
		}
	}
}
