using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Upgrades/Collector Beam Upgrade")]
public class CollectorBeamUpgrade : PlayerUpgrade {
    public CollectorBeamUpgrade() {
        UpgradeName = "Collector Beam Upgrade";
    }

    public override bool UnlockCollectorBeam(bool retVal) {
        return true;
    }
}
