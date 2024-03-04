using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drill Upgrades/Drill Tier Upgrade")]
public class DrillTierUpgrade : DrillUpgrade {
    public DrillTierUpgrade() {
        UpgradeName = "Drill Tier Upgrade";
    }

    // TODO: use to block access to later upgrades via UI
}
