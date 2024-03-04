using System.Collections.Generic;
using UnityEngine.Events;


public struct EventStruct {
	public UnityEvent e;
	public int references;
}

public class GlobalEvents {
	private static Dictionary<string, EventStruct> _eventEmitters;

	private static Dictionary<string, EventStruct> eventEmitters {
		get {
			if (_eventEmitters == null) _eventEmitters = new Dictionary<string, EventStruct>();
			return _eventEmitters;
		}
	}
	
	
	private static EventStruct GetEvent(string name) {
		if (!eventEmitters.ContainsKey(name)) {
			var newEvent = new EventStruct();
			newEvent.e = new UnityEvent();
			newEvent.references = 0;
			eventEmitters.Add(name, newEvent);
		}

		return eventEmitters[name];
	}

	public static void Emit(string eventName) {
		if (eventEmitters.TryGetValue(eventName, out var @struct)) {
			@struct.e.Invoke();
		}
	}


	public static void RegisterListener(string eventName, UnityAction action) {
		if (eventName == null) return;
		
		var @event = GetEvent(eventName);
		@event.e.AddListener(action);
		@event.references++;
	}


	public static void UnregisterListener(string eventName, UnityAction action) {
		if (eventName == null) return;
		
		var @event = GetEvent(eventName);
		@event.e.RemoveListener(action);
		@event.references--;

		if (@event.references <= 0) {
			eventEmitters.Remove(eventName);
		}
	}
}