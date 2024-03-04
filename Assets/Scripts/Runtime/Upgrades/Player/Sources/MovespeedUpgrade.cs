using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Upgrades/Move Speed Upgrade")]
public class MovespeedUpgrade : PlayerUpgrade {
    public float MoveSpeedMultiplier = 2;

    public MovespeedUpgrade() {
        UpgradeName = "Move Speed Upgrade";
    }

    public override float ModifySpeed(float retVal) {
        return retVal * MoveSpeedMultiplier;
    }
}