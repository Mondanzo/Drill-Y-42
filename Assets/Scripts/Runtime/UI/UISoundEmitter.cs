using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UISoundEmitter : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler {

	public EventReference hoverEvent;
	public EventReference clickEvent;
	
	public void OnPointerClick(PointerEventData eventData) {
		if (!clickEvent.IsNull) {
			var eventInstance = RuntimeManager.CreateInstance(clickEvent);
			eventInstance.start();
			eventInstance.release();
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (!hoverEvent.IsNull) {
			var eventInstance = RuntimeManager.CreateInstance(hoverEvent);
			eventInstance.start();
			eventInstance.release();
		}
	}
}