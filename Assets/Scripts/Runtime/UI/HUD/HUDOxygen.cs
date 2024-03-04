using UnityEngine;
using UnityEngine.UI;


public class HUDOxygen : MonoBehaviour {
	public GameObject player;
	public GameObject oxygenBar;

	private Image oxygenBarImage;

	private PlayerOxygen playerOxygen;

	private void Start() {
		oxygenBarImage = oxygenBar.GetComponent<Image>();
		playerOxygen = player.GetComponent<PlayerOxygen>();
	}

	private void Update() {
		oxygenBarImage.fillAmount = Mathf.Clamp(playerOxygen.Oxygen / playerOxygen.MaxOxygen, 0, 1);
	}
}