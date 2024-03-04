using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Team04Fig : MonoBehaviour {

	public List<GameObject> possibleArmors;
	private int index = 0;
	private Animator animator;

	private void Start() {
		animator = GetComponent<Animator>();
		SwapArmor();
	}

	public void SwapArmor() {
		animator.SetTrigger("swap");
		int newIdx = index;
		while(newIdx == index) newIdx = Random.Range(0, possibleArmors.Count);
		index = newIdx;
		for (int i = 0; i < possibleArmors.Count; i++) {
			possibleArmors[i].SetActive(i == index);
		}
	}
}