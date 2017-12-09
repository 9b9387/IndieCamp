using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class TestCharator : MonoBehaviour {

	NavAgent agent;
	// Use this for initialization
	void Start () {
		agent = GetComponent<NavAgent> ();
		EventManager.Instance.AddListener (HandyEvent.EventType.player_action, OnPlayerAction);
	}
	
	void OnPlayerAction(EventArgs args){
		StandardInputType type = args.GetValue<StandardInputType> ();
		if (type == StandardInputType.mouse_left_button_down){
			Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			agent.SetDestination (pos);
		}
	}
}
