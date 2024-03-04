using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Upgrades/Mining Efficiency Upgrade")]
public class MiningEfficiencyUpgrade : PlayerUpgrade {
    public float OreMiningMultiplier = 2;

    public MiningEfficiencyUpgrade() {
        UpgradeName = "Mining Efficiency Upgrade";
    }

    public override float ModifyOreMultiplier(float retVal) {
        return retVal * OreMiningMultiplier;
    }
}
