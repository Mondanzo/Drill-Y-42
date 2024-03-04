using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade : ScriptableObject {
	public List<UpgradeCost> upgradeCosts = new List<UpgradeCost>();
	public string UpgradeName;
	[TextArea]
	public string UpgradeDescription;
	public string Id;
    public Sprite UpgradeImage;
	public UpgradeType upgradeType;
}

[Serializable]
public class Upgrades {
	public string id;
	public bool unlocked;
	public bool isSpecial;
	public List<UpgradeCost> upgradeCosts;
}

[Serializable]
public struct UpgradeCost {
	public ItemData item;
	public int value;
}
