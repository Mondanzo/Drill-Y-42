using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public enum DistanceBarMode {
	Hidden,
	Shrunk,
	ShrunkButSave,
	ExpandedWithCollapsable,
}

public class HUDDistanceBar : MonoBehaviour
{
    public GameObject player;
    public GameObject drill;
	
	[SerializeField]
    private GameObject Tunnel;
	
	private float distance;
	private float distanceFromSpawn;

	[SerializeField]
    private GameObject playerElement;
	private RectTransform playerElementRectTransform;

	[SerializeField] 
	private GameObject sliderElement;
	private RectTransform sliderElementRectTransform;
	private float sliderElementStartWidth;
	
	[SerializeField] GameObject distanceBar;
	
	//texts and icons
	[SerializeField] TextMeshProUGUI drillMeterElement;
	[SerializeField] GameObject destructibleWallElement;
	[SerializeField] TextMeshProUGUI destructibleWallMeterElement;

	private DrillTunnelGenerator tunnelGenerator;

	private float starterPosition;
	
	private DistanceBarMode distanceBarBehavior = DistanceBarMode.Hidden;

	private void Start() {
		sliderElementStartWidth = sliderElementRectTransform.rect.width;
		UpdateBarState(0);
	}

	void OnEnable()
    {
		tunnelGenerator = Tunnel.GetComponent<DrillTunnelGenerator>();
		
		playerElementRectTransform = playerElement.GetComponent<RectTransform>();
		sliderElementRectTransform = sliderElement.GetComponent<RectTransform>();
		
		//tunnelGenerator.OnCollapsingStateChanged += UpdateBarState;
		
		starterPosition = playerElementRectTransform.position.x;
    }

	private void OnDisable() {
		//tunnelGenerator.OnCollapsingStateChanged -= UpdateBarState;
	}

	private IEnumerator canvasGroupTransparencyModifier(CanvasGroup group, float value, bool instant = false) {
		if (instant) {
			group.alpha = value;
			yield break;
		}
		
		while (group.alpha != value) {
			group.alpha = Mathf.Lerp(group.alpha, value, 0.1f);
			yield return null;
		}
	}

	public void UpdateBarState(int stateBehaviour) {
		DistanceBarMode behavior = (DistanceBarMode) stateBehaviour;
		distanceBarBehavior = behavior;
		
		GameObject background = sliderElement.transform.Find("Background").gameObject;

		switch (behavior) {
			case DistanceBarMode.Hidden:
				sliderElementRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 20f);
				StartCoroutine(canvasGroupTransparencyModifier(distanceBar.GetComponent<CanvasGroup>(), 0f, true));
				StartCoroutine(canvasGroupTransparencyModifier(destructibleWallElement.GetComponent<CanvasGroup>(), 0f, true));
				break;
			case DistanceBarMode.Shrunk:
				sliderElementRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 20f);
	
				if (background) {
					StartCoroutine(canvasGroupTransparencyModifier(background.GetComponent<CanvasGroup>(), 0f));
				}
				
				StartCoroutine(canvasGroupTransparencyModifier(distanceBar.GetComponent<CanvasGroup>(), 1f));
				StartCoroutine(canvasGroupTransparencyModifier(destructibleWallElement.GetComponent<CanvasGroup>(), 0f));
				break;
			case DistanceBarMode.ShrunkButSave:
				sliderElementRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 20f);
	
				if (background) {
					StartCoroutine(canvasGroupTransparencyModifier(background.GetComponent<CanvasGroup>(), 0f));
				}
				
				StartCoroutine(canvasGroupTransparencyModifier(distanceBar.GetComponent<CanvasGroup>(), 1f));
				StartCoroutine(canvasGroupTransparencyModifier(destructibleWallElement.GetComponent<CanvasGroup>(), 0f));
				break;
			case DistanceBarMode.ExpandedWithCollapsable:
				sliderElementRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sliderElementStartWidth);
				
				if (background) {
					StartCoroutine(canvasGroupTransparencyModifier(background.GetComponent<CanvasGroup>(), 1f));
				}
				
				StartCoroutine(canvasGroupTransparencyModifier(distanceBar.GetComponent<CanvasGroup>(), 1f));
				StartCoroutine(canvasGroupTransparencyModifier(destructibleWallElement.GetComponent<CanvasGroup>(), 1f));
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(behavior), behavior, null);
		}
	}
	
    void Update() {
		if (player == null || drill == null) return;
		distance =
            (Vector3.Distance(
                new Vector3(0, 0, player.transform.position.z),
                drill.transform.position
            ))
            +
            Vector3.Distance(
                new Vector3(0, 0, player.transform.position.z),
                tunnelGenerator.GetNextDestructibleWallPoint()
            );
		distanceFromSpawn = Vector3.Distance(
			new Vector3(0, 0, player.transform.position.z),
			new Vector3(0, 0, -73.83f)
		);

		var calculatedDistance = Mathf.Clamp(distance * 7, -(sliderElementRectTransform.rect.width / 2), 0 + (sliderElementRectTransform.rect.width / 2));

		if (distanceBarBehavior == DistanceBarMode.ExpandedWithCollapsable) {
			if (drill.GetComponent<DrillController>().drillDistance - Mathf.Clamp(tunnelGenerator.GetNextCollapsingWallPoint().z, 0, Mathf.Infinity) < 100 && tunnelGenerator.GetNextCollapsingWallPoint().z > 30) {
				sliderElementRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 
					Mathf.Clamp(Mathf.Lerp(sliderElementRectTransform.rect.width, (drill.GetComponent<DrillController>().drillDistance - tunnelGenerator.GetNextCollapsingWallPoint().z) * 2.35f, 
						0.1f), 0, sliderElementStartWidth));
			}
			
			playerElement.transform.localPosition = new Vector3(
				(sliderElementRectTransform.rect.width / 2) - Mathf.Abs(distance * 7),
				playerElement.transform.localPosition.y,
				playerElement.transform.localPosition.z
			);
		}
		    
		destructibleWallMeterElement.text = Mathf.Floor(Mathf.Clamp(tunnelGenerator.GetNextCollapsingWallPoint().z, 0, Mathf.Infinity)).ToString() + "m";
		drillMeterElement.text = Mathf.Floor(drill.GetComponent<DrillController>().drillDistance).ToString() + "m";  
		
		TextMeshProUGUI playerMeterElement = playerElement.GetComponentInChildren<TextMeshProUGUI>();
		playerMeterElement.text = Mathf.Floor(distanceFromSpawn).ToString() + "m";
	}
}
