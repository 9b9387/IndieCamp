using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class WoodFire : MonoBehaviour {
	
	public GameObject fire;

	void Start () {
		EventManager.Instance.AddListener (HandyEvent.EventType.mouse_click_object, OnClick);
	}

	void OnClick(EventArgs args){
		ClickInfo info = args.GetValue<ClickInfo> ();
		if (info.Type == StandardInputType.mouse_left_button_down){
			if (info.ClickedObject == gameObject){
				fire.SetActive (!fire.activeInHierarchy);
				if (fire.activeInHierarchy){
					EventManager.Instance.PushEvent (HandyEvent.EventType.fire_active, null);
				}
			}
		}
	}
}
