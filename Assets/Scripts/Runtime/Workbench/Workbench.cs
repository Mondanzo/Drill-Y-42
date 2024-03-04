using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Workbench : MonoBehaviour {
    public void ToggleDrillUpgradeMenu(GameObject other) {	
		print("ToggleDrillUpgradeMenu");
		var res = other.GetComponentInChildren<UpgradeUI>();
        if (res) {
            if (res.isOpen()) {
                res.Close();
            } else {
                res.Open();
            }
        }
    }

    public void TogglePlayerUpgradeMenu(GameObject other) {
        var res = other.GetComponentInChildren<UpgradeUI>();
        if (res) {
            if (res.isOpen()) {
                res.Close();
            } else {
                res.Open();
            }
        }
    }
}
