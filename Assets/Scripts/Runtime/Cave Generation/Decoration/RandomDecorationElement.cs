using System;
using System.Collections.Generic;
using UnityEngine;


[Icon("Assets/Gizmos/RandomDecorationElement Icon.png")]
public class RandomDecorationElement : MonoBehaviour {
	public List<Biome> Biome;

	public bool doClearanceCheck = true;

	[Range(0, 1)] public float SpawnRarity;

	[NonSerialized] public bool isGroup = false;

	public virtual void PlaceDecoration(bool fromGroup = false) {
		if (isGroup && !fromGroup) return;

		var isClear = true;

		if (doClearanceCheck) {
			
			foreach (var col in GetComponentsInChildren<Collider>()) {
				col.enabled = false;
			}
			
			foreach (var element in GetComponentsInChildren<Renderer>()) {
				var aabb = element.localBounds;

				if (Physics.CheckBox(element.transform.position + aabb.center, aabb.size, element.transform.rotation)) {
					isClear = false;
					break;
				}
			}
		}

		if (!isClear) {
			DestroyImmediate(gameObject);
			return;
		}
		
		gameObject.SetActive(isClear);

		if (gameObject.activeSelf && doClearanceCheck) {
			foreach (var col in GetComponentsInChildren<Collider>()) {
				col.enabled = true;
			}
		}
	}

	public void OnDrawGizmos() {
		Gizmos.DrawIcon(transform.position, "RandomDecorationElement Icon.png", true);
	}
}