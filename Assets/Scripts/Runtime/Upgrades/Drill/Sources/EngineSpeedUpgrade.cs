using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drill Upgrades/Engine Speed Upgrade")]
public class EngineSpeedUpgrade : DrillUpgrade {
    public float speedMultiplier = 1.5f;

    public EngineSpeedUpgrade() {
        UpgradeName = "Engine Speed Upgrade";
    }

    public override float ModifyBaseSpeed(float retVal) {
        return retVal * speedMultiplier;
    }
}
