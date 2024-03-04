
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Icon("Assets/Gizmos/RandomDecorationElementGroup Icon.png")]
public class RandomDecorationElementGroup : RandomDecorationElement {

	public List<RandomDecorationElement> DecoElements;
	private float count = 0f;

	public override void PlaceDecoration(bool fromGroup = false) {
		if (isGroup && !fromGroup) return;

		foreach (var decorationElement in DecoElements) {
			if(decorationElement == null) continue;
			count += Mathf.Clamp01(decorationElement.SpawnRarity);
			decorationElement.isGroup = true;
			decorationElement.gameObject.SetActive(false);
		}
		
		var target = Random.Range(0, count);
		var prev = 0f;
		foreach (var decorationElement in DecoElements) {
			if (decorationElement == null) continue;
			prev += Mathf.Clamp01(decorationElement.SpawnRarity);
			if (target <= prev) {
				// Hit the right one
				decorationElement.PlaceDecoration( true);

				if (!decorationElement) {
					DestroyImmediate(gameObject);
				} else {
					gameObject.SetActive(true);
				}
				return;
			}
		}
	}

	public new void OnDrawGizmos() {
		Gizmos.DrawIcon(transform.position, "RandomDecorationElementGroup Icon.png", true);
	}
}