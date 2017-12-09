using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class DogAI : MonoBehaviour {

	public float DESTANCE = 5.0f;

	Selector root;


	// Use this for initialization
	void Start () {
		Condition hasMeatNear = new Condition (HasMeatNear);
		Condition isCanMove = new Condition (IsCanMove);
		Action move_action = new Action (MoveToNearestBait);
		Sequence move_seq = new Sequence (hasMeatNear, isCanMove, move_action);

		root = new Selector (move_seq);

	}
	
	// Update is called once per frame
	void Update () {
		root.Execute ();
	}

	bool HasMeatNear() {
		
		List<GameObject> objs = World.Instance.GetModels ();

		foreach(GameObject obj in objs) {
			if (obj.tag == "meat") { 
				Vector3 vec3 = obj.transform.position;
				return DESTANCE > Vector3.Distance (transform.position, vec3);
			}
		}

		return false;
	}

	bool IsCanMove() {
		GameObject minDistanceObj = null;
		float distance = 0;
		GameObject[] objs = FindObjectsOfType<GameObject> ();
		foreach(GameObject obj in objs) {
			if (gameObject == obj) {
				continue;
			}
			Vector3 vec3 = obj.transform.position;
			float dis = Vector3.Distance (transform.position, vec3);
			if (distance == 0 || distance > dis)
			{
				minDistanceObj = obj;
				distance = dis;
			}
		}

		if(minDistanceObj == null){
			return false;
		}

		if (distance < 1) {
			return false;
		}
			
		return true;
	}

	Result MoveToNearestBait() {

		GameObject minDistanceObj = null;
		float distance = 0;
		GameObject[] objs = FindObjectsOfType<GameObject> ();
		foreach(GameObject obj in objs) {
			Vector3 vec3 = obj.transform.position;
			float dis = Vector3.Distance (transform.position, vec3);
			if (distance == 0 || distance > dis)
			{
				minDistanceObj = obj;
			}
		}

		if(minDistanceObj == null){
			return Result.failure;
		}

		Vector3 dir = (minDistanceObj.transform.position - transform.position).normalized;
		transform.position += dir * 4 * Time.deltaTime;

//		transform.position = Vector3.MoveTowards (transform.position, minDistanceObj.transform.position, Time.deltaTime);

		return Result.success;
	}
}
