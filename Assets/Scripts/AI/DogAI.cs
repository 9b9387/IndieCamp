using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using HandyEvent;

public class DogAI : MonoBehaviour {

	public float DESTANCE = 5.0f;

	Selector root;
	NavAgent agent;

	bool isFire = false;

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_active, OnFireActive);
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_deactive, OnFireDeactive);

		agent = GetComponent<NavAgent> ();

		Sequence chase = GetChaseSquence ();

		root = new Selector (chase);
	}
	
	// Update is called once per frame
	void Update () {
		root.Execute ();
	}

	// 点火事件
	void OnFireActive(EventArgs args){
		GameObject fire = args.GetValue<GameObject>();
		if (fire) {
			isFire = fire.activeInHierarchy;
		}
	}

//	float attackTimer = 0;
//	public float attackTime = 2f;

	// 灭火事件
	void OnFireDeactive(EventArgs args){

		if (isFire) {
			isFire = false;
		}
	}

	// 是否已经点火
	bool IsFire() {
		return isFire;
	}

	Sequence GetChaseSquence() {

		Condition isFire = new Condition (IsFire);
		Condition isInRange = new Condition (IsInRange);
		Action chase = new Action (Chase);
		return new Sequence (isFire, isInRange, chase);
	}

	bool IsInRange() {
		List<GameObject> objs = World.Instance.GetModels ();

		foreach (GameObject obj in objs) {
			if (obj.tag == "Human") {
				float dis = Vector3.Distance (transform.position, obj.transform.position);

				if(dis != 0 && dis < DESTANCE) {
					return true;
				}
			}
		}
		return false;
	}

	Result Chase() {

		List<GameObject> objs = World.Instance.GetModels ();

		float distance = 0;
		GameObject nearestHuman = null;
		foreach (GameObject obj in objs) {
			if (obj.tag == "Human") {
				float dis = Vector3.Distance (transform.position, obj.transform.position);
				if (distance == 0 || distance > dis) {
					distance = dis;
					nearestHuman = obj;
				}
			}
		}

		if (nearestHuman) {
			agent.SetDestination (new Vector2(nearestHuman.transform.position.x, nearestHuman.transform.position.y));

		}
		
		return Result.success;
	}

}
