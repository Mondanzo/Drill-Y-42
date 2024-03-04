using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour {
    [SerializeField]
    private float playerSpeed = 10f;

    [SerializeField]
    private float oreMultiplier = 1;

    [SerializeField]
    private float miningSpeedMultiplier = 1;

    [SerializeField]
    private float sprintSpeedFactor = 1.2f;

    private bool isCollectorBeamUnlocked = false;

    [SerializeField]
    private GameObject CollectorBeam;

    private bool isHammerToolUnlocked = false;

    [SerializeField]
    private GameObject HammerTool;

    [SerializeField]
    private float maxHealth = 100f;

    public List<PlayerUpgrade> PlayerUpgrades = new List<PlayerUpgrade>();

    public float PlayerSpeed {
        get {
            var retVal = playerSpeed;
            foreach (var upgrade in PlayerUpgrades) {
                retVal = upgrade.ModifySpeed(retVal);
            }
            return retVal;
        }
        set { playerSpeed = value; }
    }

    public float OreMultiplier {
        get {
            var retVal = oreMultiplier;
            foreach (var upgrade in PlayerUpgrades) {
                retVal = upgrade.ModifyOreMultiplier(retVal);
            }
            return retVal;
        }
        set { oreMultiplier = value; }
    }

    public float MiningSpeedMultiplier {
        get {
            var retVal = miningSpeedMultiplier;
            foreach (var upgrade in PlayerUpgrades) {
                retVal = upgrade.ModifyMiningSpeed(retVal);
            }
            return retVal;
        }
        set { miningSpeedMultiplier = value; }
    }

    public float SprintSpeedFactor { get => sprintSpeedFactor; set => sprintSpeedFactor = value; }

    public bool IsCollectorBeamUnlocked {
        get {
            var retVal = isCollectorBeamUnlocked;
            foreach (var upgrade in PlayerUpgrades) {
                retVal = upgrade.UnlockCollectorBeam(retVal);
            }
            return retVal;
        }
        set { isCollectorBeamUnlocked = value; }
    }

    public bool IsHammerToolUnlocked {
        get {
            var retVal = isHammerToolUnlocked;
            foreach (var upgrade in PlayerUpgrades) {
                retVal = upgrade.UnlockHammerTool(retVal);
            }
            return retVal;
        }
        set { isHammerToolUnlocked = value; }
    }

    public float MaxHealth {
        get {
            var retVal = maxHealth;
            foreach (var upgrade in PlayerUpgrades) {
                retVal = upgrade.ModifyMaxHealth(retVal);
            }
            return retVal;
        }
        set { maxHealth = value; }
    }
}
