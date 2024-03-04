using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Upgrades/Mining Speed Upgrade")]
public class MiningSpeedUpgrade : PlayerUpgrade {
    public float MiningSpeedMultiplier = 2;

    public MiningSpeedUpgrade() {
        UpgradeName = "Mining Speed Upgrade";
    }

    public override float ModifyMiningSpeed(float retVal) {
        return retVal * MiningSpeedMultiplier;
    }
}
