using UnityEngine;

[CreateAssetMenu(menuName = "Player Upgrades/Hammer Tool Upgrade")]
public class HammerToolUpgrade : PlayerUpgrade {
    HammerToolUpgrade() {
        UpgradeName = "Hammer Tool Upgrade";
    }

    public override bool UnlockHammerTool(bool retVal) {
        return true;
    }
}
