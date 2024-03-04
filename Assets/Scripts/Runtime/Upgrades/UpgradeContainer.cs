using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType {
	Drill,
	Player
}

public enum UpgradeCategory {
	Solo,
	Multi
}

[CreateAssetMenu(menuName = "Upgrade Container")]
public class UpgradeContainer : ScriptableObject
{
	public Upgrade mainUpgrade;
	public List<UpgradeListing> Enhancements = new List<UpgradeListing>();
	public bool dontIncludeMainUpgrade;
	public UpgradeType upgradeType;
}

[Serializable]
public struct UpgradeListing {
	public List<Upgrade> upgrades;
	public string name;
}