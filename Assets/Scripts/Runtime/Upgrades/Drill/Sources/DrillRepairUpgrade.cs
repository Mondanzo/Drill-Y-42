using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drill Upgrades/Repair Drill Upgrade")]
public class DrillRepairUpgrade : DrillUpgrade {
        
    public DrillRepairUpgrade() {
        UpgradeName = "Repair Drill";
    }

    internal override bool ChangeDrillRepairStatus(bool retVal) {
        return true;
    }
}
