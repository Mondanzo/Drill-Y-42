using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDHealth : MonoBehaviour {
	public GameObject player;
	public GameObject healthBar;
	public GameObject healthText;
	
	private RectTransform healthBarRectTransform;
	private TextMeshProUGUI healthTextMeshPro;
	
	private PlayerHealth playerHealth;

	private void Start() {
		healthBarRectTransform = healthBar.GetComponent<RectTransform>();
		playerHealth = player.GetComponent<PlayerHealth>();
		healthTextMeshPro = healthText.GetComponent<TextMeshProUGUI>(); 
	}

	private void Update() {
		healthTextMeshPro.text = Mathf.Floor(Mathf.Clamp(playerHealth.Health, 0, 100)).ToString();
		healthBarRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Clamp(playerHealth.Health * 2, 0, 500));
	}
}
