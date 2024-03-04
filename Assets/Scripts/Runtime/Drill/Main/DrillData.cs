using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DrillData : MonoBehaviour {

	[Header("Base Stats")]
    [SerializeField]
    private float maxGear = 1;

    [SerializeField]
    private float baseSpeed = 1.0f;

    [SerializeField]
    private float baseFuelConsumption = 1.0f;

    [SerializeField]
    private bool isDrillRepaired = false;

	[SerializeField]
	private float maxFuel = 500f;

	[Header("Active Upgrades")]
	public List<DrillUpgrade> DrillUpgrades = new List<DrillUpgrade>();

	[HideInInspector]
	public UnityEvent<DrillUpgrade> UpgradeAdded;
	public void AddUpgrade(DrillUpgrade upgrade) {
		DrillUpgrades.Add(upgrade);
		UpgradeAdded.Invoke(upgrade);
	}

	public float MaxGear {
        get {
            var retVal = maxGear;
            foreach (var upgrade in DrillUpgrades) {
                retVal = upgrade.ModifyMaxGear(retVal);
            }
            return retVal;
        }
        set => maxGear = value;
    }

    public float BaseSpeed {
        get {
            var retVal = baseSpeed;
            foreach (var upgrade in DrillUpgrades) {
                retVal = upgrade.ModifyBaseSpeed(retVal);
            }
            return retVal;
        }
        set => baseSpeed = value;
    }

    public float BaseFuelConsumption {
        get {
            var retVal = baseFuelConsumption;
            foreach (var upgrade in DrillUpgrades) {
                retVal = upgrade.ModifyBaseFuelConsumption(retVal);
            }
            return retVal;
        }
        set => baseFuelConsumption = value;
    }

    public bool IsDrillRepaired {
        get {
            var retVal = isDrillRepaired;
            foreach (var upgrade in DrillUpgrades) {
                retVal = upgrade.ChangeDrillRepairStatus(retVal);
            }
            return retVal;
        }
        set => isDrillRepaired = value;
    }

	public float MaxFuel {
		get {
			var retVal = maxFuel;
			foreach (var upgrade in DrillUpgrades) {
				retVal = upgrade.ModifyMaxFuel(retVal);
			}
			return retVal;
		}
		set => maxFuel = value;
	}
}
