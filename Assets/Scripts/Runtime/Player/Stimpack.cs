using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Stimpack : MonoBehaviour {
	private PlayerInputs playerInputs;

	private PlayerItems playerItems;
	private PlayerHealth playerHealth;
	private StudioEventEmitter studioEventEmitter;

	[Header("References")]
	[SerializeField]
	private ItemData healingComponent;

	[Header("Settings")]
	public int maxDoses = 2;
	public int doses = 0;

	[SerializeField]
	[Range(0, 100)]
	private int healthRestorePercentage = 50;
	[SerializeField]
	[Range(0, 100)]
	private int oxygenRestorePercentage = 50;

	[SerializeField] private GameObject HUD;
	private HUDStim HUDStimComponent;

	public UnityEvent OnUseStimpack;
	
	public delegate void StimpackUsedEvent();
	public event StimpackUsedEvent StimpackUsed;
	
	private void Awake() {
		playerInputs = new PlayerInputs();
		playerInputs.Interactions.Stimpack.performed += onPerformed;
	}

	void Start() {
		playerItems = GetComponentInParent<PlayerItems>();
		playerHealth = GetComponentInParent<PlayerHealth>();
		studioEventEmitter = GetComponent<StudioEventEmitter>();
		HUDStimComponent = HUD.GetComponent<HUDStim>(); 
	}

	private void onPerformed(InputAction.CallbackContext context) {
		useStimpack();
	}

	private void addHUDStimpack(ItemInstance data, List<ItemInstance> items) {
		if (data.itemType == healingComponent) {
			HUDStimComponent.addStimpackBar();
		}
	}
	
	private void useStimpack() {
		if (doses > 0) {
			doses--;
			var healAmount = playerHealth.MaxHealth * (healthRestorePercentage / 100f);
			playerHealth.Heal(healAmount);

			HUDStimComponent.removeStimpack();
			if(OnUseStimpack != null) OnUseStimpack.Invoke();
			if(StimpackUsed != null) StimpackUsed.Invoke();
			
			// TODO: add oxygen heal

			studioEventEmitter.Play();
		}
	}

	private void Update() {
		if (playerItems.inventory.GetItemQuantity(healingComponent) >= 3 && doses < maxDoses) {
			// create 1 stimpack use
			
			doses++;
			// remove the three healing plants
			playerItems.inventory.RemoveItem(healingComponent, 3);
		}
	}

	public bool StimpacksFull() {
		return doses >= maxDoses;
	}

	private void OnEnable() {
		playerInputs.Interactions.Stimpack.Enable();
		
		playerItems = GetComponentInParent<PlayerItems>();
		Inventory.onItemAdded += addHUDStimpack;
	}

	private void OnDisable() {
		playerInputs.Interactions.Stimpack.Disable();
	}
}
