using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drill Upgrades/Max Gear Upgrade")]
public class MaxGearUpgrade : DrillUpgrade {

    public float maxGearMultiplier = 3;

    public MaxGearUpgrade() {
        UpgradeName = "Max Gear Upgrade";
    }

    public override float ModifyMaxGear(float retVal) {
        return retVal * maxGearMultiplier;
    }
}
