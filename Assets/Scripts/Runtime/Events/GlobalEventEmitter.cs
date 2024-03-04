using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GlobalEventEmitter : MonoBehaviour {
	public string eventName;
	private string finalEventName;
	
	
	private void Awake() {
		finalEventName = eventName;
	}

	
	public void Emit() {
		GlobalEvents.Emit(finalEventName);
	}
}