using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class Meet : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag ("Human") || other.CompareTag ("Dog")){
			EventManager.Instance.PushEvent (HandyEvent.EventType.hit_meet, other.gameObject);
		}
	}
}
