using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollectorBeam : MonoBehaviour {
    private PlayerInputs playerInputs;

	private StudioEventEmitter studioEventEmitter;

    [Header("References")]
    [SerializeField]
    private Animator handAnimator;

    [Header("Settings")]
    [SerializeField]
    private float audioFadeOutAmount = 1f;
    [SerializeField]
    private float vacuumForce = 50.0f;
    [SerializeField]
    private float collectDistance = 1.0f;

    private bool isCollecting = false;
	
	private void Awake() {
        playerInputs = new PlayerInputs();
        playerInputs.Interactions.Secondary.performed += onPerformed;
        playerInputs.Interactions.Secondary.canceled += onCanceled;
    }

    private void Start() {
		studioEventEmitter = GetComponent<StudioEventEmitter>();
	}

    private void onPerformed(InputAction.CallbackContext obj) {
        isCollecting = true;
        handAnimator.SetBool("FirstPersonDrillBool", true);

		studioEventEmitter.Play();
    }

    private void onCanceled(InputAction.CallbackContext obj) {
        isCollecting = false;
        handAnimator.SetBool("FirstPersonDrillBool", false);

		studioEventEmitter.Stop();
    }

	private void vacuumItems(Collider other) {
        var rigidBody = other.GetComponent<Rigidbody>();
        Vector3 relativeDirection = transform.position - other.transform.position;
        rigidBody.AddForce(relativeDirection.normalized * vacuumForce);

		if (relativeDirection.sqrMagnitude <= collectDistance) {
            var droppedItem = other.GetComponent<DroppedItem>();
            if (droppedItem) {
				droppedItem.pickUpItem();
                Destroy(other.gameObject);
            }
		}
	}

    private void OnTriggerStay(Collider other) {
        if (isCollecting) {
            // TODO: use layer instead because apparently this is the fastest comparison
            if (other.gameObject.CompareTag("Items")) {
                vacuumItems(other);
            }
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Items")) {
			vacuumItems(other);
		}
    }

    private void OnEnable() {
        playerInputs.Interactions.Secondary.Enable();
    }

    private void OnDisable() {
        playerInputs.Interactions.Secondary.Disable();
    }
}
