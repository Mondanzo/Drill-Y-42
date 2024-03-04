using System;
using UnityEngine;

[Icon("Assets/Gizmos/RandomUniqueDecorationElement Icon.png")]
public class RandomUniqueDecorationElement : MonoBehaviour, ISerializationCallbackReceiver {
	
	public Guid uniqueId;
	
	[HideInInspector]
	[SerializeField] private string uuid;
	public bool doClearanceCheck;

	public bool picked = false;

	public void PlaceUniqueDecoration() {
		picked = true;
		

		gameObject.SetActive(true);
	}


	[ContextMenu("Create Unique ID")]
	public void GenerateUniqueID() {
		uniqueId = Guid.NewGuid();
	}


	public void OnDrawGizmos() {
		Gizmos.DrawIcon(transform.position, "RandomDecorationElement Icon.png", false);

		foreach (var r in GetComponentsInChildren<Renderer>()) {
			Gizmos.matrix = r.transform.localToWorldMatrix;
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(Vector3.zero + r.localBounds.center, r.localBounds.size);
		}
	}

	public void OnBeforeSerialize() {
		uuid = uniqueId.ToString();
	}

	public void OnAfterDeserialize() {
		uniqueId = Guid.Parse(uuid);
	}
}