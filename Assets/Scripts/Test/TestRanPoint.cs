using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class TestRanPoint : MonoBehaviour {

	public GameObject obj;

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListener (HandyEvent.EventType.player_action, OnAction);
	}
	
	void OnAction(EventArgs args){
		StandardInputType type = args.GetValue<StandardInputType> ();
		if (type == StandardInputType.mouse_left_button_down){
			Vector2 pos = MapHandler.Instant.GetRandomPoint ();
			Instantiate (obj, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
		}
	}
}
