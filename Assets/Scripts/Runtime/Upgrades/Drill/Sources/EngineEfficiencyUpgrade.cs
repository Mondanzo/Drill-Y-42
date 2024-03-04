using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drill Upgrades/Engine Efficiency Upgrade")]
public class EngineEfficiencyUpgrade : DrillUpgrade {
    public float efficiencyMultiplier = 2;

    public EngineEfficiencyUpgrade() {
        UpgradeName = "Engine Efficiency Upgrade";
    }

    public override float ModifyBaseFuelConsumption(float retVal) {
        return retVal / efficiencyMultiplier;
    }
}
