﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class WoodFire : MonoBehaviour {
	
	public GameObject fire;
	public GameObject flame;

	void Start () {
		EventManager.Instance.AddListener (HandyEvent.EventType.mouse_click_object, OnClick);
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_deactive, OnFireDeactive);
	}

	void OnClick(EventArgs args){
		ClickInfo info = args.GetValue<ClickInfo> ();
		if (info.Type == StandardInputType.mouse_left_button_down){
			if (info.ClickedObject == gameObject){
				if (!fire.activeInHierarchy){
					fire.SetActive (true);
					flame.SetActive (true);
				}
				EventManager.Instance.PushEvent (HandyEvent.EventType.fire_active, fire);
			}
		}
	}

	void OnFireDeactive(EventArgs args) {
		fire.SetActive (false);
		flame.SetActive (false);

	}
}
