using FMODUnity;
using System.Collections;
using UnityEngine;

public class FuelPort : MonoBehaviour {
    private DrillController drillController;
    private PlayerItems playerItems;

	[Header("References")]
	[SerializeField]
	private StudioEventEmitter studioEventEmitter;
	[SerializeField]
	private ItemData fuelSource;
	[SerializeField]
	private Animator animator;

	[Header("Settings")]
    public float fuelReturnPerItem = 10f;

    private bool isFueling = false;
	private bool crRunning = false;
	private bool crystalAnimFinished;

    private void Start() {
        drillController = GetComponentInParent<DrillController>();
        playerItems = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerItems>();
    }

    private void Update() {
        if (isFueling) {
			if (!crRunning) {
				StartCoroutine(refuelDrill());
			}
		}
    }

	private IEnumerator refuelDrill() {
		crRunning = true;

		var crystals = playerItems.inventory.GetItemQuantity(fuelSource);
		if (crystals > 0) {
			animator.SetTrigger("Fueling");
			yield return new WaitUntil(() => crystalAnimFinished);
			crystalAnimFinished = false;
		}

		crRunning = false;
	}

	// called by animation event
	public void CrystalAnimStart() {
		if (studioEventEmitter) studioEventEmitter.Play();
		playerItems.inventory.RemoveItem(fuelSource, 1);
	}

	// called by animation event
	public void CrystalAnimFinish() {
		crystalAnimFinished = true;

		drillController.AddFuel(fuelReturnPerItem);
	}

	public void StartFueling(GameObject other) {
        if (other.CompareTag("Player")) {
            isFueling = true;
        }
    }

    public void StopFueling(GameObject other) {
        if (other.CompareTag("Player")) {
            isFueling = false;
        }
    }
}
