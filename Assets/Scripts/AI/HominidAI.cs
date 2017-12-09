using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;


public class HominidAI : MonoBehaviour {

	//World world;
		
	public GameObject campFire;

	NavAgent agent;
	Selector root;
	Vector3 targetPos;

	public float speed = 4.0f;

	bool isMoving = false;

	// Use this for initialization
	void Start () {
		
		agent = GetComponent<NavAgent> ();

		Condition isFire = new Condition (IsFire);
		Condition noMoving = new Condition (NoMoving);
		Action moveToFire_action = new Action (MoveToFire);
		Sequence move_seq = new Sequence (isFire, noMoving, moveToFire_action);

		Condition isArriveCamp = new Condition (IsArriveCamp);
		Action outfire_action = new Action (Outfire);
		Sequence outfire_seq = new Sequence (isFire, isArriveCamp, outfire_action);

		Condition noTarget = new Condition (NoTarget);
		Action newPos = new Action (NewPos);
		Sequence newPos_seq = new Sequence (noTarget, newPos);


		Action patrol = new Action (Patrol);
		Action clearPos = new Action (ClearPos);

//		Condition isMoving = new Condition (IsMoving);
		Sequence patrol_seq = new Sequence (noMoving, patrol, noMoving, clearPos);
		Selector partol = new Selector (newPos_seq, patrol_seq);
//		Condition isArrive = new Condition (IsArriveCamp);

		root = new Selector (outfire_seq, move_seq, partol);
	}
	
	// Update is called once per frame
	void Update () {
		root.Execute ();
	}

	bool NoTarget() {
		return targetPos.x == 0 && targetPos.y == 0;
	}

	bool IsFire() {
		return campFire.activeInHierarchy;
	}

	bool NoMoving() {
		return !agent.IsMoving;
	}

	bool IsMoving() {
		return agent.IsMoving;
	}

	bool IsArriveCamp() {
		if (campFire) {
			float dis = Vector2.Distance (campFire.transform.position, transform.position);
			Debug.Log (dis);
			return dis <= 1.2;
		}

		return false;
	}

	Result MoveToFire() {
		if (campFire) {
			agent.SetDestination (campFire.transform.position);
		}

		return Result.success;
	}

	Result Outfire() {
		Debug.Log ("out fire");
		if (campFire && campFire.activeInHierarchy) {
			campFire.SetActive (false);
		}
		return Result.success;
	}

	Result Patrol() {
		agent.SetDestination (targetPos);
		return Result.success;
	}

	Result NewPos() {
		int offsetX = Random.Range (3, 5);
		int offsetY = Random.Range (3, 5);

		targetPos = new Vector3 (transform.position.x + offsetX, transform.position.y + offsetY, transform.position.z);
		return Result.success;
	}

	Result ClearPos() {
		targetPos = new Vector3 (0, 0, 0);

		return Result.success;
	}
}
