using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class InputHandler : Singleton<InputHandler> {

	public LayerMask clickableMask;

	Camera m_cam;

	void Start () {
		m_cam = Camera.main;
		if (!m_cam){
			Debug.LogError ("no camera !!!!");
		}
	}

	void Update () {
		Vector2 mousePos = m_cam.ScreenToWorldPoint (Input.mousePosition);
		if (Input.GetMouseButtonDown (0)) {
			Collider2D coll = Physics2D.OverlapPoint (mousePos, clickableMask);
			if (coll){
				EventManager.Instance.PushEvent (HandyEvent.EventType.mouse_click_object, coll.gameObject);
			}
		}

		EventManager.Instance.PushEvent (HandyEvent.EventType.mouse_position, mousePos);
	}
}
