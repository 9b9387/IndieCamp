using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using HandyEvent;


public class HominidAI : MonoBehaviour {

	//World world;
		
	GameObject campFire;

	NavAgent agent;
	Animator animator;

	Selector root;
	Vector3 moveTarget;

	bool isFire = false;
	bool isStart = false;

	float lastPosX = 0.0f;

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_active, OnFireActive);
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_deactive, OnFireDeactive);
		
		agent = GetComponent<NavAgent> ();
		animator = GetComponent<Animator> ();
		Sequence outFire = GetOutFireSeq ();
		Selector move = GetMoveSelector ();
		Sequence partol = GetPartolSequence();
		root = new Selector (outFire, move, partol);

		animator.SetBool ("Run", true);
	}

	// 点火事件
	void OnFireActive(EventArgs args){
		GameObject fire = args.GetValue<GameObject>();
		if (fire) {
			isFire = fire.activeInHierarchy;
			campFire = fire;
		}
	}

	float attackTimer = 0;
	public float attackTime = 2f;

	// 灭火事件
	void OnFireDeactive(EventArgs args){
			
		if (isFire && campFire) {
			campFire.SetActive (false);
			isFire = false;
			agent.speed = 1.0f;
			attackTimer = 0;
		}

		animator.SetBool ("Ext", false);

	}
		
	public void StartAI() {
		isStart = true;
	}
		
	// Update is called once per frame
	void Update () {
		if (isStart) {
			root.Execute ();
		}


		UpdateDir ();
	}

	void UpdateDir() {
		// 向右走
		if (gameObject.transform.position.x - lastPosX > 0) {
			// 转向右
			gameObject.transform.localScale = new Vector3 (Mathf.Abs (transform.localScale.x) * -1, transform.localScale.y, transform.lossyScale.z);
		}
		// 向左走
		else if (gameObject.transform.position.x - lastPosX < 0) {
			// 转向左
			gameObject.transform.localScale = new Vector3 (Mathf.Abs (transform.localScale.x), transform.localScale.y, transform.lossyScale.z);
		}
		lastPosX = gameObject.transform.position.x;
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
//			Debug.Log (dis);
			return dis <= 1.0;
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
		attackTimer += Time.deltaTime;

		if (transform.localScale.x > 0) {
			transform.localScale = new Vector3 (Mathf.Abs (transform.localScale.x), transform.localScale.y, transform.localScale.z);
		} else {
			transform.localScale = new Vector3 (Mathf.Abs (transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
		}

		if (campFire && campFire.activeInHierarchy) {
//			campFire.SetActive (false);

			if (attackTimer < attackTime) {
				animator.SetBool ("Ext", true);
				return Result.running;
			}

			if (isFire && campFire) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.fire_deactive, null);
				return Result.success;

			}

		} else {
			
		}
		return Result.failure;
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
		Condition isMoving = new Condition (IsMoving);
		Condition noMoving = new Condition (NoMoving);
		Action stop = new Action (Stop);
		Action action = new Action (SetMoveTarget);

		Selector stopSelector = new Selector (noMoving, new Sequence(isMoving, stop));

		return new Sequence (isFire, stopSelector, action);
	}

	Result Stop() {
		agent.StopMove ();

		return Result.success;
	}

	// 设置移动目标为火堆
	Result SetMoveTarget() {
//		Debug.Log ("setMovetarget" + campFire);
		if (campFire) {
			agent.speed = 5.0f;
			animator.SetBool ("Run", false);
			agent.SetDestination (campFire.transform.position);
//			Debug.Log (agent.IsMoving);
		}

		return Result.success;
	}
	// end of Sequence Node

	// 移动的Sequence Node
	Selector GetMoveSelector() {
		Sequence moveToFire = GetMoveToCampFireSeq ();
		return new Selector (moveToFire);
	}

	// 灭火的Sequence Node
	Sequence GetOutFireSeq() {
		Condition isFire = new Condition (IsFire);
		Condition isArriveCamp = new Condition (IsArriveCamp);

		Condition isMoving = new Condition (IsMoving);
		Condition noMoving = new Condition (NoMoving);
		Action stop = new Action (Stop);

		Selector stopSelector = new Selector (noMoving, new Sequence(isMoving, stop));

		Action outfire_action = new Action (Outfire);
		return new Sequence (isFire, isArriveCamp, stopSelector, outfire_action);
	}

	// 巡逻的Sequence Node 
	Sequence GetPartolSequence() {
		Condition noMoving = new Condition (NoMoving);
		Action action = new Action (Parto);

		return new Sequence (noMoving, action);
	}

	Result Parto() {
		Vector2 target = MapHandler.Instant.GetRandomPoint ();
		agent.speed = 1.0f;
		animator.SetBool ("Run", false);
		agent.SetDestination (target);

		return Result.success;
	}
}
