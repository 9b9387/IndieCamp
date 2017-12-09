using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class Pit : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){

		Debug.Log ("Pit OnTriggerEnter2D");
		if (other.gameObject.CompareTag ("Human") || other.gameObject.CompareTag ("Dog")){
			EventManager.Instance.PushEvent (HandyEvent.EventType.hit_pit, other.gameObject);
		}
	}
}
