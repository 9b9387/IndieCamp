using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using HandyEvent;


public class HominidAI : MonoBehaviour {

	//World world;
		
	GameObject campFire;

	NavAgent agent;
	Selector root;
	Vector3 moveTarget;

	public float speed = 4.0f;

	bool isMoving = false;


	bool isFire = false;
	bool isInStage = false;

	bool isStart = false;

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_active, OnFireActive);
		
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

//		root = new Selector (outfire_seq, move_seq, partol);



		Sequence movdToCampFire = GetMoveToCampFireSeq ();
		root = new Selector (movdToCampFire);
	}

	// 点火事件
	void OnFireActive(EventArgs args){
		GameObject fire = args.GetValue<GameObject>();
		if (fire) {
			isFire = fire.activeInHierarchy;
			campFire = fire;
		}

//		Debug.Log ("OnFireActive" + isFire + " " + campFire);

	}

	public void StartAI() {
		isStart = true;
	}
		
	// Update is called once per frame
	void Update () {
		if (isStart) {
			root.Execute ();
		}
	}

	bool NoTarget() {
		return moveTarget.x == 0 && moveTarget.y == 0;
	}

	// 是否已经点火
	bool IsFire() {
		return isFire;
	}

	bool NoMoving() {
//		Debug.Log (agent.IsMoving);
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
		agent.SetDestination (moveTarget);
		return Result.success;
	}

	Result NewPos() {
		int offsetX = Random.Range (3, 5);
		int offsetY = Random.Range (3, 5);

		moveTarget = new Vector3 (transform.position.x + offsetX, transform.position.y + offsetY, transform.position.z);
		return Result.success;
	}

	Result ClearPos() {
		moveTarget = new Vector3 (0, 0, 0);

		return Result.success;
	}

	// 获取移动到篝火的Sequence Node
	Sequence GetMoveToCampFireSeq() {
		Condition isFire = new Condition (IsFire);
		Condition noMoving = new Condition (NoMoving);
		Action action = new Action (SetMoveTarget);

		return new Sequence (isFire, noMoving, action);
	}

	// 设置移动目标为火堆
	Result SetMoveTarget() {
		Debug.Log ("setMovetarget" + campFire);
		if (campFire) {
			agent.SetDestination (campFire.transform.position);
			Debug.Log (agent.IsMoving);
		}

		return Result.success;
	}
}
