using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class TestNavAgent : MonoBehaviour {

	NavAgent navAgent;
	// Use this for initialization
	void Start () {
		navAgent = GetComponent<NavAgent> ();
		EventManager.Instance.AddListener (HandyEvent.EventType.player_action, OnPlayerAction);
	}
	
	void OnPlayerAction(EventArgs args){
		StandardInputType type = args.GetValue<StandardInputType> ();
		if (type == StandardInputType.mouse_left_button_down){
			Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			navAgent.SetDestination (pos);
		}
	}
}
