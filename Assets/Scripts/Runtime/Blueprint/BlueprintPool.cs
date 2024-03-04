using System.Collections.Generic;
using UnityEngine;	

[CreateAssetMenu(menuName = "Blueprint Pool")]
public class BlueprintPool : ScriptableObject
{
    [SerializeField]
    private int tier = 1;

	public List<UpgradeContainer> unlockableUpgradeContainers;
	public List<Upgrade> unlockableUpgrades;
}
