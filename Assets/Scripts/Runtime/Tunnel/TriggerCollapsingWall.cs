using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerTriggerZone))]
[RequireComponent(typeof(DrillTriggerZone))]
public class TriggerCollapsingWall : MonoBehaviour {

	private bool drillOut = false;
	private bool playerOut = false;

	public DrillTunnelSegment targetSegment;

	// Start is called before the first frame update
	void Start() {
		var playerZone = GetComponent<PlayerTriggerZone>();
		playerZone.PlayerStay.AddListener(PlayerEntered);
		playerZone.PlayerExited.AddListener(PlayerExited);
		
		var drillZone = GetComponent<DrillTriggerZone>();
		drillZone.DrillStay.AddListener(DrillEntered);
		drillZone.DrillExited.AddListener(DrillExited);
	}

	private void DrillEntered() {
		drillOut = false;
	}

	private void DrillExited() {
		drillOut = true;
		CheckIfSuccess();
	}

	private void PlayerEntered() {
		playerOut = false;
	}

	private void PlayerExited() {
		playerOut = true;
		CheckIfSuccess();
	}

	void CheckIfSuccess() {
		if (playerOut && drillOut) {
			// Time to lock em out of the Safe cave.
			var result = GetComponentInParent<DrillTunnelGenerator>();

			if (result) {
				result.TeleportCollapsingWall(targetSegment);
				result.SetCollapsing(true);
			}
		}
	}
}