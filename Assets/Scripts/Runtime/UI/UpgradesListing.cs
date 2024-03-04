/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradesListing : MonoBehaviour
{
    public List<PlayerUpgrade> upgradesList;
    public GameObject uiPrefab;
    public GameObject uiPrefabList;
    public GameObject playerUpgradeUi;
    
    public Sprite upgradedSprite;
    
    private GameObject player;
    private PlayerData playerData;
    private PlayerItems playerItems;
    public ItemInformation itemInformation;

    private List<GameObject> uiElements = new List<GameObject>();

    void Start()
    {
        playerUpgradeUi.SetActive(false);
        player = GameObject.Find("Player");
        playerData = player.GetComponent<PlayerData>();
        playerItems = player.GetComponent<PlayerItems>();
        
        createUpgradeMenu();
    }
    
    private void createUpgradeMenu() {
        for (int i = 0; i < upgradesList.Count; i++)
        {
            PlayerUpgrade upgrade = upgradesList[i];
            
            GameObject ui = Instantiate(uiPrefab, uiPrefabList.transform);
            ui.name = upgrade.UpgradeName;
            
            RectTransform Rect = ui.GetComponent<RectTransform>();

            Rect.anchoredPosition = new Vector2(0, -85 + -i * 50);

            GameObject text = ui.transform.Find("Title").gameObject;

            text.GetComponent<TMPro.TextMeshProUGUI>().text = upgrade.UpgradeName + " (" + upgrade.CrystalsNeeded.ToString() + ")";
            
            uiElements.Add(ui);
        }

        updateMenu();
    }
    
    private void updateMenu()
    {
        for (int i = 0; i < upgradesList.Count; i++) {
            PlayerUpgrade upgrade = upgradesList[i];
            GameObject ui = uiElements[i];

            if (playerData.PlayerUpgrades.Contains(upgrade))
            {
                GameObject image = ui.transform.Find("Image").gameObject;
                image.GetComponent<Image>().sprite = upgradedSprite;
            }

            if (playerItems.inventory.GetItem(itemInformation.getItemData("Crystal")) >= upgrade.CrystalsNeeded && !playerData.PlayerUpgrades.Contains(upgrade))
            {
                GameObject button = ui.transform.Find("Button").gameObject;

                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { 
                    if (playerItems.inventory.GetItem(itemInformation.getItemData("Crystal")) >= upgrade.CrystalsNeeded && !playerData.PlayerUpgrades.Contains(upgrade)) {
                        playerItems.inventory.RemoveItem(itemInformation.getItemData("Crystal"), upgrade.CrystalsNeeded);
                        playerData.PlayerUpgrades.Add(upgrade);
                        updateMenu();
                    }
                });
                button.gameObject.GetComponent<Button>().interactable = true;
            }
            else
            {
                GameObject button = ui.transform.Find("Button").gameObject;
                button.GetComponent<Button>().interactable = false;
            }
        }
    }

    private void Update()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu() {
        if (!playerUpgradeUi.active) {
            playerUpgradeUi.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;

            createUpgradeMenu();
        }
        else {
            foreach (GameObject ui in uiElements) {
                Destroy(ui);
            }
            uiElements.Clear();
            playerUpgradeUi.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
*/
