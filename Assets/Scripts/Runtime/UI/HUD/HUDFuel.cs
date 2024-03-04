using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDFuel : MonoBehaviour
{
	[SerializeField]
    private GameObject drill;

	private DrillController drillController;
	
	[SerializeField]
	private GameObject fuelBar;
	private Image fuelBarImage;

	void Start() {
		drillController = drill.GetComponent<DrillController>();
		fuelBarImage = fuelBar.GetComponent<Image>();
	}

	void Update() {
		if (drillController == null || fuelBarImage == null) return;
		float fuelPercentage = drillController.Fuel / drillController.MaxFuel;
		fuelBarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(fuelBarImage.rectTransform.rect.height, fuelPercentage * 115, 0.1f));
	}
}
