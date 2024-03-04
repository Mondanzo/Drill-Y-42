using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDPrompt : MonoBehaviour {
	[SerializeField]
	private GameObject promptPrefab;
	[SerializeField]
	private Sprite buttonImage;

	private GameObject promptInstance;

	private void Start() {
		promptInstance = Instantiate(promptPrefab, gameObject.transform);
		promptInstance.SetActive(false);
		promptInstance.GetComponentInChildren<Image>().sprite = buttonImage;
	}

	private IEnumerator hide() {
		yield return null;
		promptInstance.SetActive(false);
	}

	public void Show() {
		StopAllCoroutines();

		promptInstance.SetActive(true);

		StartCoroutine(hide());
	}
}