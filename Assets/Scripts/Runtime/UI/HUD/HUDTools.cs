	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;


	[Serializable]
	public struct ToolList {
		public Sprite icon;
		public Sprite keyIcon;
		public Upgrade upgrade;
		public string toolName;
	}

	public class HUDTools : MonoBehaviour
	{   
	    [SerializeField] private GameObject toolsBarParent;
		[SerializeField] private GameObject toolsBar;
		[SerializeField] private List<ToolList> tools = new List<ToolList>();

		private GameObject player;
		private PlayerData playerData;
		private GameObject upgradeUIGameObject;
		private UpgradeUI upgradeUI;

		private List<string> toolsBarList = new List<string>();
		
		private void Start() {
			SwitchTools.onEquip += Equip;

			player = GameObject.FindGameObjectWithTag("Player");
			playerData = player.GetComponent<PlayerData>();
			upgradeUIGameObject = player.transform.Find("UserInterface/UpgradeUI").gameObject;
			upgradeUI = upgradeUIGameObject.GetComponent<UpgradeUI>();
			
			upgradeUI.onUpgradeUnlocked += onUpgradeUnlockedEvent;

			foreach (Upgrade upgrade in playerData.PlayerUpgrades) {
				//check if upgrade is in tools list
				foreach (ToolList tool in tools) {
					if (upgrade == tool.upgrade) {
						updateToolbar();
					}
				}
			}
		}
		
		private void updateToolbar() {
			foreach (Upgrade upgrade in playerData.PlayerUpgrades) {
				foreach (ToolList toolList in tools) {
					if (toolList.upgrade == upgrade && !toolsBarList.Contains(toolList.toolName)) {
						GameObject newTool = Instantiate(toolsBar, toolsBarParent.transform);
						newTool.name = toolList.toolName;
					
						Image IconImage = newTool.transform.Find("Image").GetComponent<Image>();
						IconImage.sprite = toolList.icon;
					
						Image KeyIconImage = newTool.transform.Find("KeyButton").GetComponent<Image>();
						KeyIconImage.sprite = toolList.keyIcon;
						
						toolsBarList.Add(toolList.toolName);
					}
				}
			}
		}

		private void onUpgradeUnlockedEvent(Upgrade upgrade) {
			updateToolbar();
		}

		private void Equip(Tool tool) {
		if (toolsBarParent == null) return;

			foreach (Transform child in toolsBarParent.transform) {
				if (child.name == tool.name) {
					Image IconImage = child.transform.Find("Image").GetComponent<Image>();
					IconImage.color = new Color(0.38f, 0.38f, 0.38f);
				} else {
					Image IconImage = child.transform.Find("Image").GetComponent<Image>();
					IconImage.color = Color.white;
				}
			}
		}
	}
