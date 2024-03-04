using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Upgrades/Gauntlet Tier Upgrade")]
public class GauntletTierUpgrade : PlayerUpgrade {
    public GauntletTierUpgrade() {
        UpgradeName = "Gauntlet Tier Upgrade";
    }

    // TODO: use to block access to later upgrades via UI
}
