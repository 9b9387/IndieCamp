using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using HandyEvent;


public class HominidAI : MonoBehaviour {

	//World world;

	public float MEAT_RANGE = 5.0f;
		
	GameObject campFire;

	NavAgent agent;
	Animator animator;

	Selector root;
	Vector3 moveTarget;

	bool isFire = false;
	bool isStart = false;
	bool isHitPit = false;
	bool isHitMeat = false;

	float lastPosX = 0.0f;

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_active, OnFireActive);
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_deactive, OnFireDeactive);
		EventManager.Instance.AddListener (HandyEvent.EventType.hit_pit, OnHitPit);
		EventManager.Instance.AddListener (HandyEvent.EventType.hit_meet, OnHitMeat);

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
			isHitPit = false;
			isHitMeat = false;
		}

		animator.SetBool ("Ext", false);
	}


	void OnHitPit(EventArgs args) {
		Debug.Log ("OnHitPit");
		GameObject obj = args.GetValue<GameObject> ();

		if (obj == gameObject && isFire) {
			Debug.Log ("isHitPit");

			isHitPit = true;
		}
	}

	void OnHitMeat(EventArgs args) {
		GameObject obj = args.GetValue<GameObject> ();

		if (obj == gameObject && isFire) {
			isHitMeat = true;
		}
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
//		Debug.Log ("out fire");
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
		Sequence pit = GetPitSequence ();
		Sequence meat = GetMeatSequence ();
		Sequence moveToFire = GetMoveToCampFireSeq ();
		return new Selector (pit, meat, moveToFire);
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
		Vector2 target = MapHandler.Instant.GetRandomPoint (true);
		agent.speed = 1.0f;
		animator.SetBool ("Run", false);
		agent.SetDestination (target);

		return Result.success;
	}


	// 道具行为

	bool IsHitPit() {
		return isHitPit;
	}

	Sequence GetPitSequence() {
		Condition isFire = new Condition (IsFire);
		Condition isPit = new Condition (IsHitPit);
		Action stop = new Action (Stop);
		//Action todo;

		return new Sequence (isFire, isPit, stop);
	}

	bool IsFoundMeat() {
		List<GameObject> objs = World.Instance.GetModels ();

//		float distance = 0;
		foreach (GameObject obj in objs) {
			if (obj.tag == "Meet") {
				float dis = Vector3.Distance (transform.position, obj.transform.position);
//				if (distance == 0 || distance > dis) {
//					distance = dis;
//				}
				Debug.Log(dis + " " + MEAT_RANGE + " " + (dis < MEAT_RANGE));
				if(dis != 0 && dis < MEAT_RANGE) {
					return true;
				}
			}
		}

		return false;
	}

	Result MoveToMeat() {
		Debug.Log ("MoveToMeat");
		List<GameObject> objs = World.Instance.GetModels ();

		float distance = 0;
		GameObject nearestMeat = null;
		foreach (GameObject obj in objs) {
			if (obj.tag == "Meet") {
				float dis = Vector3.Distance (transform.position, obj.transform.position);
				if (distance == 0 || distance > dis) {
					distance = dis;
					nearestMeat = obj;
				}
			}
		}

		if (nearestMeat) {
			animator.SetBool ("Run", true);
			agent.speed = 5.0f;
			agent.SetDestination (new Vector2(nearestMeat.transform.position.x, nearestMeat.transform.position.y));
		}

		return Result.success;
	}

	bool IsArriveMeat() {
//		Debug.Log ("IsArriveMeat");
//		List<GameObject> objs = World.Instance.GetModels ();
//
//		float distance = 0;
//		GameObject nearestMeat = null;
//		foreach (GameObject obj in objs) {
//			if (obj.tag == "Meet") {
//				float dis = Vector3.Distance (transform.position, obj.transform.position);
//				if (distance == 0 || distance > dis) {
//					distance = dis;
//					nearestMeat = obj;
//				}
//			}
//		}
//
//		if (nearestMeat) {
//			float dis = Vector2.Distance (nearestMeat.transform.position, transform.position);
//			Debug.Log (dis);
//					
//			return dis <= 1.0;
//		}
		Debug.Log(isHitMeat);
		return isHitMeat;
	}

	Result EatMeat() {
		//		Debug.Log ("out fire");
		attackTimer += Time.deltaTime;

		if (attackTimer < attackTime) {
			animator.SetBool ("Ext", true);
			return Result.running;
		}

		animator.SetBool ("Ext", false);
		return Result.success;
	}

	Sequence GetMeatSequence() {
		Condition isFire = new Condition (IsFire);

		Condition isArriveMeat = new Condition (IsArriveMeat);
		Action eatMeat = new Action (EatMeat);
		Condition isFoundMeat = new Condition (IsFoundMeat);
		Action stop = new Action (Stop);
		Action moveToMeat = new Action (MoveToMeat);

		Selector eat = new Selector(new Sequence(isArriveMeat, eatMeat), new Sequence(isFoundMeat, stop, moveToMeat));

		return new Sequence (isFire, eat);
	}
}
