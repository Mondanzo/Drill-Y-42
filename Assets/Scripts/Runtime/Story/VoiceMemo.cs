using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interaction))]
public class VoiceMemo : MonoBehaviour {

	public VoiceMemoOrders voiceMemosSet;

	void Start() {
		

		var interaction = GetComponent<Interaction>();
		interaction.Executed.AddListener(PickupMemo);
	}

	private void Update() {
		if (voiceMemosSet.Exists()) {
			gameObject.SetActive(voiceMemosSet.HasMemosLeft());
		} else {
			Destroy(gameObject);
		}
	}

	private void PickupMemo(GameObject pickup) {
		Destroy(gameObject);

		SubtitlePlayer.PlayClip(voiceMemosSet.GetNextMemo());
	}
}