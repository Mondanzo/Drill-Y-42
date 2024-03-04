/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DrillUpgradesListing : MonoBehaviour {
    public List<DrillUpgrade> upgradesList;
    public GameObject uiPrefab;
    public GameObject uiPrefabList;
    public GameObject UpgradeUi;

    public Sprite upgradedSprite;

    private GameObject drill;
    private DrillData drillData;
    private PlayerItems playerItems;
    public ItemInformation itemInformation;
    private List<GameObject> uiElements = new List<GameObject>();

    void Start() {
        UpgradeUi.SetActive(false);
        drill = GameObject.Find("Drill");
        drillData = drill.GetComponent<DrillData>();        
        playerItems = GameObject.Find("Player").GetComponent<PlayerItems>();

        createUpgradeMenu();
    }

    private void createUpgradeMenu() {
        for (int i = 0; i < upgradesList.Count; i++) {
            DrillUpgrade upgrade = upgradesList[i];

            GameObject ui = Instantiate(uiPrefab, uiPrefabList.transform);
            ui.name = upgrade.UpgradeName;

            RectTransform Rect = ui.GetComponent<RectTransform>();

            Rect.anchoredPosition = new Vector2(0, -85 + -i * 50);

            GameObject text = ui.transform.Find("Title").gameObject;

            text.GetComponent<TMPro.TextMeshProUGUI>().text = upgrade.UpgradeName + " (" + upgrade.CrystalsNeeded.ToString() + ")";

            uiElements.Add(ui);
        }
    }

    private void updateMenu() {
        for (int i = 0; i < upgradesList.Count; i++) {
            DrillUpgrade upgrade = upgradesList[i];
            GameObject ui = uiElements[i];

            if (drillData.DrillUpgrades.Contains(upgrade))
            {
                GameObject image = ui.transform.Find("Image").gameObject;
                image.GetComponent<Image>().sprite = upgradedSprite;
            }

            if (playerItems.inventory.GetItem(itemInformation.getItemData("Crystal")) >= upgrade.CrystalsNeeded && !drillData.DrillUpgrades.Contains(upgrade)) {
                GameObject button = ui.transform.Find("Button").gameObject;
                button.GetComponent<Button>().interactable = true;

                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                    if (playerItems.inventory.GetItem(itemInformation.getItemData("Crystal")) >= upgrade.CrystalsNeeded && !drillData.DrillUpgrades.Contains(upgrade)) {
                        playerItems.inventory.RemoveItem(itemInformation.getItemData("Crystal"), upgrade.CrystalsNeeded);
                        drillData.DrillUpgrades.Add(upgrade);
                        updateMenu();
                        
                    }
                });
            }
            else {
                GameObject button = ui.transform.Find("Button").gameObject;
                button.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void ToggleMenu() {
        if (!UpgradeUi.active) {
            UpgradeUi.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;

            updateMenu();
        }
        else {
            UpgradeUi.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Update() {
        if (Keyboard.current.jKey.wasPressedThisFrame) {
            ToggleMenu();
        }
    }
}
*/
