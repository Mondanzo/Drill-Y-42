using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using FMODUnity;

//disclaimer: mit purem schmerz geschrieben
//Finger weg von Scriptable Objects kann ich nur dazu sagen

[Serializable]
public struct headerButton {
	public GameObject button;
	public UpgradeType category;
}

public class UpgradeUI : MonoBehaviour, PlayerInputs.IUIActions {
	[SerializeField] private GameObject containerPanel;
	private Vector3 containerPanelVector = Vector3.zero;

	[SerializeField] private GameObject header;
	[SerializeField] private GameObject body;

	[SerializeField] private GameObject enhancementPrefab;
	[SerializeField] private GameObject enhancementLevelPrefab;
	[SerializeField] private GameObject itemDetailPrefab;
	[SerializeField] private GameObject upgradeCategoryButtonPrefab;
	
	//sounds
	[SerializeField] private StudioEventEmitter unlockGauntletSound;
	[SerializeField] private StudioEventEmitter unlockDrillSound;
	[SerializeField] private StudioEventEmitter errorUpgradeSound;
	[SerializeField] private StudioEventEmitter buttonSound;
	
	public List<UpgradeContainer> upgradeContainers;
	public ItemInformation itemInformation;
	public GameObject drill;
	
	[SerializeField] private Sprite nodeUpgradedSprite;
	
	[SerializeField] private List<headerButton> headerButtons = new List<headerButton>();
	private List<GameObject> upgradeCategoryButtons = new List<GameObject>();

	private UpgradeType currentCategory = UpgradeType.Drill;
	
	private PlayerMovement playerMovement;
	private PlayerInputs playerInputs;
	private PlayerItems playerItems;
	private PlayerData playerData;
	
	private List<Upgrade> unlockedUpgrades = new List<Upgrade>();
	
	private DrillData drillData;
	private CanvasGroup canvasGroup;

	public delegate void OnUpgradeUnlocked(Upgrade upgrade);
	public event OnUpgradeUnlocked onUpgradeUnlocked;

	private void OnEnable() {
		var player = GameObject.Find("Player");
		playerMovement = player.GetComponent<PlayerMovement>();
		playerItems = player.GetComponent<PlayerItems>();
		playerData = player.GetComponent<PlayerData>();
		
		drillData = drill.GetComponent<DrillData>();
		
		setupHeader();

		if (playerInputs == null) {
			playerInputs = new PlayerInputs();

			playerInputs.UI.AddCallbacks(this);
			playerInputs.UI.Enable();
		}
		
		canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;
	}

	private IEnumerator modifyCanvasGroup(float targetAlpha, float duration) {
		float time = 0;
		float startAlpha = canvasGroup.alpha;
		float distance = targetAlpha - startAlpha;
		
		while (time < duration) {
			time += Time.deltaTime;
			float alpha = Mathf.Sin(time / duration * (Mathf.PI / 2)) * distance + startAlpha;
			canvasGroup.alpha = alpha;
			yield return null;
		}
		canvasGroup.alpha = targetAlpha;
	}
	
	private IEnumerator shakeTransform(RectTransform rectTransform, float duration, float magnitude) {
		float time = 0;
		if (containerPanelVector == Vector3.zero) {
			containerPanelVector = rectTransform.localPosition;
		}
		while (time < duration) {
			time += Time.deltaTime;
			float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
			float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;
			rectTransform.localPosition = containerPanelVector + new Vector3(x, y, 0);
			yield return null;
		}
		rectTransform.localPosition = containerPanelVector;
	}
	
	private void Start() {
		selectCategory(UpgradeType.Drill);
		Close();
	}
	
	public void Open() {
		if (upgradeContainers.Count == 0) {
			errorUpgradeSound.Play();
			return;
		}
		
		selectCategory(currentCategory);
		UnityEngine.Cursor.lockState = CursorLockMode.Confined;
		StartCoroutine(modifyCanvasGroup(1, 0.1f));

		PauseScreen.canPause = false;
		//playerInputs.PlayerMovement.Disable();
		playerMovement.Disable();
	}

	public void Close() {
		UnityEngine.Cursor.lockState = CursorLockMode.Locked;
		
		setupHeader();
		StartCoroutine(modifyCanvasGroup(0, 0.1f));

		PauseScreen.canPause = true;
		//playerInputs.PlayerMovement.Enable();
		playerMovement.Enable();
	}
	
	public bool isOpen() {
		return canvasGroup.alpha == 1;
	}

	static IEnumerator inlineMove(RectTransform inlineTransform, float targetX, float sizeX, float duration) {
		float time = 0;
		float startX = inlineTransform.anchoredPosition.x;
		float startSizeX = inlineTransform.sizeDelta.x;
		float distance = targetX - startX;
		
		while (time < duration) {
			time += Time.deltaTime;
			float x = Mathf.Sin(time / duration * (Mathf.PI / 2));
			inlineTransform.anchoredPosition = Vector2.Lerp(new Vector2(startX, inlineTransform.anchoredPosition.y), new Vector2(targetX, inlineTransform.anchoredPosition.y), x);
			inlineTransform.sizeDelta = Vector2.Lerp(new Vector2(startSizeX, inlineTransform.sizeDelta.y), new Vector2(sizeX, inlineTransform.sizeDelta.y), x);	
			yield return null;
		}
	}

	public void setupHeader() {
		GameObject inline = header.transform.Find("Inline").gameObject;
		
		foreach (headerButton hButton in headerButtons) {
			Button buttonComponent = hButton.button.GetComponent<Button>();
			buttonComponent.onClick.RemoveAllListeners();

			if (currentCategory == hButton.category) {
				StartCoroutine(inlineMove(inline.GetComponent<RectTransform>(), hButton.button.GetComponent<RectTransform>().anchoredPosition.x, hButton.button.GetComponent<RectTransform>().sizeDelta.x, 1));
			}

			buttonComponent.onClick.AddListener(
				delegate {
					buttonSound.Play();
					selectCategory(hButton.category);
					StartCoroutine(inlineMove(inline.GetComponent<RectTransform>(), hButton.button.GetComponent<RectTransform>().anchoredPosition.x, hButton.button.GetComponent<RectTransform>().sizeDelta.x, 0.1f));
				});
		}
	}
	
	public void selectCategory(UpgradeType category) {
		foreach (GameObject button in upgradeCategoryButtons) {
			Destroy(button);
		}
		upgradeCategoryButtons.Clear();
		
		currentCategory = category;
		UpgradeContainer currentContainer = null;
		
		foreach (UpgradeContainer container in upgradeContainers) {
			if (container.upgradeType == category) {
				GameObject button = Instantiate(upgradeCategoryButtonPrefab, body.transform.Find("Scroll View/Viewport/Content"));
				button.transform.Find("Icon").gameObject.GetComponent<Image>().sprite = container.mainUpgrade.UpgradeImage;
				button.GetComponent<Button>().onClick.AddListener(delegate { buttonSound.Play(); selectUpgrade(container.mainUpgrade.Id, category); });
				upgradeCategoryButtons.Add(button);

				if (currentContainer == null) {
					currentContainer = container;
				}
			}
		}

		if (currentContainer) {
			selectUpgrade(!currentContainer.dontIncludeMainUpgrade ? currentContainer.mainUpgrade.Id : "EmptyNoUpgradeNoNoNoNoUpgradeNope", category);
		} else {
			selectUpgrade("EmptyNoUpgradeNoNoNoNoUpgradeNope", category);
		}
	}
	
	public void selectUpgrade(string upgradeId, UpgradeType category) {
		var upgradeObject = upgradeContainers.Find(x => x.mainUpgrade.Id == upgradeId);
		
		if (upgradeObject) {
			TextMeshProUGUI Title = body.transform.Find("Panel/Title").gameObject.GetComponent<TextMeshProUGUI>();
			TextMeshProUGUI Description = body.transform.Find("Panel/Description").gameObject.GetComponent<TextMeshProUGUI>();
			Image Image = body.transform.Find("Panel/Icon").gameObject.GetComponent<Image>();
			
			var mainupgrade = upgradeObject.mainUpgrade;
			
			Title.text = mainupgrade.UpgradeName;
			Description.text = mainupgrade.UpgradeDescription;
			Image.sprite = mainupgrade.UpgradeImage;

			foreach (Transform child in body.transform.Find("Panel/ItemsRequiredField").transform) {
				if (child.gameObject.name != "Title")
					GameObject.Destroy(child.gameObject);
			}

			Button upgradeButton = body.transform.Find("Panel/UpgradeButton").gameObject.GetComponent<Button>();
			if (!upgradeObject.dontIncludeMainUpgrade) {
				if (mainupgrade && unlockedUpgrades.Contains(mainupgrade)) {
					upgradeButton.interactable = false;
					upgradeButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "Unlocked";
				} else {
					upgradeButton.interactable = true;
					upgradeButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "Upgrade";
				}
				
				body.transform.Find("Panel/UpgradeButton").gameObject.SetActive(true);
				upgradeButton.onClick.RemoveAllListeners();
				upgradeButton.onClick.AddListener(delegate {  unlockMainUpgrade(upgradeButton, mainupgrade.Id); });
				
				body.transform.Find("Panel/ItemsRequiredField").gameObject.SetActive(true);
				
				List<UpgradeCost> itemsRequired = mainupgrade.upgradeCosts;
				foreach (UpgradeCost itemRequired in itemsRequired) {
					GameObject itemDetail = Instantiate(itemDetailPrefab, body.transform.Find("Panel/ItemsRequiredField"));
					itemDetail.GetComponent<Image>().sprite = itemRequired.item.itemImage;
					itemDetail.transform.Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text = itemRequired.value.ToString();
				}
			} else {
				body.transform.Find("Panel/UpgradeButton").gameObject.SetActive(false);
				body.transform.Find("Panel/ItemsRequiredField").gameObject.SetActive(false);
			}
			
			
			GameObject enhancementContainer = body.transform.Find("Panel/EnhancementContainer/Viewport/Content").gameObject;
			foreach (Transform child in enhancementContainer.transform) {
				GameObject.Destroy(child.gameObject);
			}
			
			foreach (UpgradeListing list in upgradeObject.Enhancements) {
				GameObject enhancementPrefabCopy = Instantiate(enhancementPrefab, enhancementContainer.transform);
				enhancementPrefabCopy.transform.Find("Title").gameObject.GetComponent<TextMeshProUGUI>().text = list.name;
				
				RectTransform enhancementRectTransform = enhancementPrefabCopy.GetComponent<RectTransform>();
				enhancementRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 95.5f);
				enhancementRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 292.86f);

				GameObject enhancementButton = enhancementPrefabCopy.transform.Find("Button").gameObject;
				Button button = enhancementButton.GetComponent<Button>();
				button.onClick.RemoveAllListeners();
				
				var atEnhancementLevel = GetCurrentEnhancementLevel(list);

				button.interactable = atEnhancementLevel < list.upgrades.Count;
				button.onClick.AddListener(delegate { unlockEnhancement(button, list.upgrades, mainupgrade); });

				if(atEnhancementLevel < list.upgrades.Count) {
					var currentEnhancementUpgrade = list.upgrades[atEnhancementLevel];
					currentEnhancementUpgrade.upgradeCosts.ForEach(x => {
						GameObject itemDetail = Instantiate(itemDetailPrefab, enhancementPrefabCopy.transform.Find("ItemHolder"));
						
						TextMeshProUGUI Count = itemDetail.transform.Find("Count").gameObject.GetComponent<TextMeshProUGUI>();
						
						RectTransform rectTransform = itemDetail.GetComponent<RectTransform>();
						rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.width / 1.35f);
						rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height / 1.35f);
						
						itemDetail.GetComponent<Image>().sprite = x.item.itemImage;
						Count.text = x.value.ToString();
						Count.fontSize = 10f;
					});
				}
				
				foreach (Upgrade upgrade in list.upgrades) {
					GameObject enhancementLevel = Instantiate(enhancementLevelPrefab, enhancementPrefabCopy.transform.Find("Levels"));
					enhancementLevel.GetComponent<Image>().color = unlockedUpgrades.Contains(upgrade) ? new Color(0.42f, 1f, 0.43f) : new Color(0.14f, 0.14f, 0.16f);
				}
			}
		}
	}
	private int GetCurrentEnhancementLevel(UpgradeListing list) {
		int atEnhancementLevel = 0;

		foreach (Upgrade upgrade in list.upgrades) {
			if (unlockedUpgrades.Contains(upgrade)) {
				atEnhancementLevel++;
			}
		}

		return atEnhancementLevel;
	}

	void unlockEnhancement(Button button, List<Upgrade> upgrades, Upgrade mainUpgrade) {
		var targetEnhancement = GetNextEnchancement(upgrades);

		// Make sure to not try to upgrade if we're already fully upgraded.
		if (upgrades.Count <= targetEnhancement) return;
		
		Upgrade upgradeToUnlock = upgrades[targetEnhancement];
		UpgradeType category = upgradeToUnlock.upgradeType;
		
		
		// Check if player contains the required items
		for (int i = 0; i < upgradeToUnlock.upgradeCosts.Count; i++) { 
			var requiredItem = upgradeToUnlock.upgradeCosts[i];
				
			if (playerItems.inventory.GetItemQuantity(itemInformation.getItemData(requiredItem.item.itemName)) < requiredItem.value) {
				StartCoroutine(shakeTransform(containerPanel.GetComponent<RectTransform>(), 0.2f, 10f));
				errorUpgradeSound.Play();
				return;
			}
		}
		
		
		// Take them to pay.
		for (int i = 0; i < upgradeToUnlock.upgradeCosts.Count; i++) { 
			var requiredItem = upgradeToUnlock.upgradeCosts[i];
			playerItems.inventory.RemoveItem(itemInformation.getItemData(requiredItem.item.itemName), requiredItem.value);
		}
		
		
		if (category == UpgradeType.Player) {
			playerData.PlayerUpgrades.Add(upgradeToUnlock as PlayerUpgrade);
			unlockedUpgrades.Add(upgradeToUnlock);
		} else if (category == UpgradeType.Drill) {
			drillData.DrillUpgrades.Add(upgradeToUnlock as DrillUpgrade);
			unlockedUpgrades.Add(upgradeToUnlock);
		}
		
		selectUpgrade(mainUpgrade.Id, category);
	}
	
	private int GetNextEnchancement(List<Upgrade> upgrades) {
		int targetUpgrade = 0;

		for (int i = 0; i < upgrades.Count; i++) {
			Upgrade upgrade = upgrades[i];

			if (unlockedUpgrades.Contains(upgrade)) {
				targetUpgrade = i + 1;
			}
		}

		return targetUpgrade;
	}

	void unlockMainUpgrade(Button button, string upgradeIdentifier) {
		UpgradeContainer upgradeObject = upgradeContainers.Find(x => x.mainUpgrade.Id == upgradeIdentifier);

		UpgradeType category = upgradeObject.upgradeType;
		Upgrade mainupgrade = upgradeObject.mainUpgrade;

		if (category == UpgradeType.Player) {
			for (int i = 0; i < mainupgrade.upgradeCosts.Count; i++) { 
				var requiredItem = mainupgrade.upgradeCosts[i];
				
				if (playerItems.inventory.GetItemQuantity(itemInformation.getItemData(requiredItem.item.itemName)) < requiredItem.value) {
					errorUpgradeSound.Play();
					StartCoroutine(shakeTransform(containerPanel.GetComponent<RectTransform>(), 0.2f, 10f));
					return;
				}
			}

			for (int i = 0; i < mainupgrade.upgradeCosts.Count; i++) { 
				var requiredItem = mainupgrade.upgradeCosts[i];
				playerItems.inventory.RemoveItem(itemInformation.getItemData(requiredItem.item.itemName), requiredItem.value);
			}
			
			unlockGauntletSound.Play();
			
			playerData.PlayerUpgrades.Add(upgradeObject.mainUpgrade as PlayerUpgrade);
			unlockedUpgrades.Add(upgradeObject.mainUpgrade);
			if (onUpgradeUnlocked != null) onUpgradeUnlocked(upgradeObject.mainUpgrade);
		} else if (category == UpgradeType.Drill) {
			for (int i = 0; i < mainupgrade.upgradeCosts.Count; i++) { 
				var requiredItem = mainupgrade.upgradeCosts[i];
				
				if (playerItems.inventory.GetItemQuantity(itemInformation.getItemData(requiredItem.item.itemName)) < requiredItem.value) {
					errorUpgradeSound.Play();
					StartCoroutine(shakeTransform(containerPanel.GetComponent<RectTransform>(), 0.2f, 10f));
					return;
				}
			}
			
			for (int i = 0; i < mainupgrade.upgradeCosts.Count; i++) { 
				var requiredItem = mainupgrade.upgradeCosts[i];
				playerItems.inventory.RemoveItem(itemInformation.getItemData(requiredItem.item.itemName), requiredItem.value);
			}
			
			unlockDrillSound.Play();
			
			drillData.DrillUpgrades.Add(upgradeObject.mainUpgrade as DrillUpgrade);
			unlockedUpgrades.Add(upgradeObject.mainUpgrade);
			if (onUpgradeUnlocked != null) onUpgradeUnlocked(upgradeObject.mainUpgrade);
		}
		
		selectUpgrade(mainupgrade.Id, category);
	}
	
	public void OnEscapeAction(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			if (isOpen())
			{
				Close();
			}
		}
	}

	void OnDisable() {
		playerInputs.UI.Disable();
	}
}
