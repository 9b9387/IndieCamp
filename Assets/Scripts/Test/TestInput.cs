using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class TestInput : MonoBehaviour {


	bool isClicked = false;
	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListener (HandyEvent.EventType.mouse_click_object, OnClickObject);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnClickObject(EventArgs args){
		GameObject obj = args.GetValue<GameObject> ();
		Debug.Log (obj.name);
		isClicked = !isClicked;
		if (isClicked){
			EventManager.Instance.AddListener (HandyEvent.EventType.mouse_position, OnMousePos);
		}else{
			EventManager.Instance.RemoveListener (HandyEvent.EventType.mouse_position, OnMousePos);
		}
	}

	void OnMousePos(EventArgs args){
		Vector2 pos = args.GetValue<Vector2> ();
		Debug.Log (pos);
		if (isClicked){
			transform.position = new Vector3 (pos.x, pos.y, 0);
		}
	}
}
