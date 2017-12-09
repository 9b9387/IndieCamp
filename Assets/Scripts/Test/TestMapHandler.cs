using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class TestMapHandler : MonoBehaviour {

	public GameObject block;
	public NavAgent guy;
	public Transform destination;

	Vector2 m_destination;

	void Start () {
		EventManager.Instance.AddListener (HandyEvent.EventType.player_action, OnPlayerAction);
		m_destination = new Vector2 (destination.position.x, destination.position.y);
	}
	
	void OnPlayerAction(EventArgs args){
		StandardInputType type = args.GetValue<StandardInputType> ();
		if (type == StandardInputType.mouse_right_button_down){
			GameObject obj = Instantiate<GameObject> (block);
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			obj.transform.position = new Vector3 (pos.x, pos.y, 0);
			obj.transform.rotation = Quaternion.identity;
			obj.layer = LayerMask.NameToLayer ("Block");
			MapHandler.Instant.ProcessMap ();
		}

		if (type == StandardInputType.mouse_left_button_down){
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			NavAgent agent = Instantiate<NavAgent> (guy);
			agent.transform.position = new Vector3 (pos.x, pos.y, 0);
			agent.transform.rotation = Quaternion.identity;
			agent.SetDestination (m_destination);
		}
	}
}
