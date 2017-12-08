using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace HandyEvent{

	public delegate void EventHandler(EventArgs agrs);

	public class EventManager : Singleton<EventManager> {

		private Queue<Event> events = new Queue<Event> ();
		private Dictionary<EventType, EventHandler> handlers = new Dictionary<EventType, EventHandler>();

		protected override void Awake(){
			base.Awake ();
			StartCoroutine (Execute ());
		}

		public void AddListener(EventType type, EventHandler listener){
			if (handlers.ContainsKey (type)){
				handlers [type] += listener;
			}else{
				handlers.Add (type, listener);
			}
		}

		public void RemoveListener(EventType type, EventHandler listener){
			if (handlers.ContainsKey (type)){
				handlers [type] -= listener;
			}
		}

		public void PushEvent(EventType type, object arg){
			if (!handlers.ContainsKey (type)){
				return;
			}

			Event e = new Event (type, new EventArgs (arg));

			events.Enqueue (e);
		}

		private IEnumerator Execute(){
			while (true) {
				if (events.Count > 0){
					for (int i = 0; i < events.Count; i++) {
						Event e = events.Dequeue ();
						if (handlers[e.type] != null){
							handlers [e.type] (e.agrs);
						}
					}
				}

				yield return null;
			}
		}
	}

	public class EventArgs {
		private object value;

		public EventArgs(object v){
			value = v;
		}

		public T GetValue<T>(){
			try{
				return (T)value;

			}catch (InvalidCastException ue){
				Debug.Log (ue.ToString ());
			}

			return default(T);
		}
	}

	public class Event {
		public EventType type;
		public EventArgs agrs;

		public Event(EventType _type, EventArgs _agrs){
			type = _type;
			agrs = _agrs;
		}
	}
}
