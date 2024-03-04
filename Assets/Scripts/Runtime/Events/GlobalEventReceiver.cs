using System;
using UnityEngine;
using UnityEngine.Events;


public class GlobalEventReceiver : MonoBehaviour {
	public string eventName;
	private string finalEventName;

	public UnityEvent OnEmit;

	private void Awake() {
		finalEventName = eventName;
	}

	private void OnEnable() {
		GlobalEvents.RegisterListener(finalEventName, Emit);
	}


	private void OnDisable() {
		GlobalEvents.UnregisterListener(finalEventName, Emit);
	}


	private void Emit() {
		if(OnEmit != null) OnEmit.Invoke();
	}
}