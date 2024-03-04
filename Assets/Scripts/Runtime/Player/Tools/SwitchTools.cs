using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchTools : MonoBehaviour, PlayerInputs.IToolsActions {
    private PlayerInputs playerInputs;

    [Header("References")]
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private GameObject miningTool;
    private MiningBeam miningBeam;
    [SerializeField]
    private GameObject collectorBeamTool;

    [SerializeField]
    private GameObject hammerTool;
    private HammerTool hammerToolScript;
    [SerializeField]
    private GameObject alchemyTool;

    public Tool equippedTool;

	public delegate void OnEquip(Tool tool);
	public static event OnEquip onEquip;

    private void Awake() {
        playerInputs = new PlayerInputs();
        playerInputs.Tools.AddCallbacks(this);
    }

    private void Start() {
        miningBeam = miningTool.GetComponent<MiningBeam>();
        hammerToolScript = hammerTool.GetComponent<HammerTool>();

        if (equippedTool == null) {
            equipMiningTool();
		}
    }

	private void Update() {
		// TODO: change this fix for collector beam not being set active upon unlocking the upgrade to use events maybe?
		if (equippedTool is MiningBeam) {
			if (playerData.IsCollectorBeamUnlocked && !collectorBeamTool.activeSelf) {
				collectorBeamTool.SetActive(true);
			}
		}
	}

	public void OnSelectFirstTool(InputAction.CallbackContext context) {
        if (context.performed) {
			equipMiningTool();
		}
    }

    public void OnSelectSecondTool(InputAction.CallbackContext context) {
        if (context.performed) {
			equipHammerTool();
			
			if (onEquip != null) {
				onEquip(equippedTool);
			}
        }
    }

    public void OnSelectThirdTool(InputAction.CallbackContext context) {
        if (context.performed) {
			equipAlchemyTool();
			
			if (onEquip != null) {
				onEquip(equippedTool);
			}
		}
    }

    public void OnCycleBetweenTools(InputAction.CallbackContext context) {
        if (context.performed) {

        }
    }

    private void equipMiningTool() {
        unequipAll();
        equippedTool = miningBeam;
        miningTool.SetActive(true);
        if (playerData.IsCollectorBeamUnlocked) {
            collectorBeamTool.SetActive(true);
        }
		
		if (onEquip != null) {
			onEquip(equippedTool);
		}
    }

    private void equipHammerTool() {
        if (playerData.IsHammerToolUnlocked) {
            unequipAll();
            equippedTool = hammerToolScript;
            if (playerData.IsHammerToolUnlocked) {
                hammerTool.SetActive(true);
            }
        }
    }

    private void equipAlchemyTool() {

    }

    private void unequipAll() {
        miningTool.SetActive(false);
        collectorBeamTool.SetActive(false);

        hammerTool.SetActive(false);

        //alchemyTool.SetActive(false);
    }

    private void OnEnable() {
        playerInputs.Tools.Enable();
    }

    private void OnDisable() {
        playerInputs.Tools.Disable();
    }
}
