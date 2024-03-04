using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomBlueprint : MonoBehaviour {
    [SerializeField]
    private BlueprintPool bluePrintPool;

    private UpgradeUI upgradesListing;

    private void Start() {
        var uiGameObjects = GameObject.FindGameObjectsWithTag("UI");

        // TODO: use the interaction event to receive the player gameobject and get all the lists from there instead of loading it at start like right now
        foreach (var uiGameObject in uiGameObjects) {
            if (upgradesListing == null) {
                uiGameObject.TryGetComponent<UpgradeUI>(out upgradesListing);
            }
		}
    }

    public void Add() {
        var availableUpgrades = getNotYetUnlockedUpgrades(bluePrintPool.unlockableUpgradeContainers);

        var upgrade = getRandomUpgrade(availableUpgrades);

        if (upgrade != null) {
            addUpgrade(upgrade);

			GameObject player = GameObject.Find("Player");
			GameObject UserInterface = player.transform.Find("UserInterface").gameObject;
			GameObject HUD = UserInterface.transform.Find("HUD").gameObject;
			Popup popup = HUD.GetComponent<Popup>();
			
			popup.PopupEvent("Unlocked Upgrade: " + upgrade.name, 3f);
        }
        else {
            // TODO: work out what should happen if player already has all upgrades for the tier pool of upgrades
            print("player already has found all blueprint upgrades");
			
			GameObject player = GameObject.Find("Player");
			GameObject UserInterface = player.transform.Find("UserInterface").gameObject;
			GameObject HUD = UserInterface.transform.Find("HUD").gameObject;
			Popup popup = HUD.GetComponent<Popup>();
			
			popup.PopupEvent("All upgrades unlocked. For now...", 3f);
        }
        Destroy(gameObject);
    }

    private List<UpgradeContainer> getNotYetUnlockedUpgrades(List<UpgradeContainer> unlockableUpgrades) {
		return unlockableUpgrades.Where(upgrade => !upgradesListing.upgradeContainers.Contains(upgrade)).ToList();
	}

    private UpgradeContainer getRandomUpgrade(List<UpgradeContainer> upgrades) {
        if (upgrades.Count > 0) {
            var index = UnityEngine.Random.Range(0, upgrades.Count);
            return upgrades[index];
        }
        return null;
    }

    private void addUpgrade(UpgradeContainer upgrade) {
        print("unlocked upgrade: " + upgrade.name);
		upgradesListing.upgradeContainers.Add(upgrade);
    }
}
