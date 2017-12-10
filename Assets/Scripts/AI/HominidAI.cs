using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using HandyEvent;


public class HominidAI : MonoBehaviour {

	//World world;

	public float MEAT_RANGE = 5.0f;
	public float DOG_RANGE = 3.0f;
		
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

	void OnDestroy() {
		EventManager.Instance.RemoveListener (HandyEvent.EventType.fire_active, OnFireActive);
		EventManager.Instance.RemoveListener (HandyEvent.EventType.fire_deactive, OnFireDeactive);
		EventManager.Instance.RemoveListener (HandyEvent.EventType.hit_pit, OnHitPit);
		EventManager.Instance.RemoveListener (HandyEvent.EventType.hit_meet, OnHitMeat);
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
		}

		agent.speed = 1.0f + RandomSpeed();
		attackTimer = 0;
		isHitPit = false;
		isHitMeat = false;

		animator.SetBool ("Ext", false);
		animator.SetBool ("Run", false);

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
			agent.speed = 5.0f + RandomSpeed();
			animator.SetBool ("Run", true);
			agent.SetDestination (campFire.transform.position);
//			Debug.Log (agent.IsMoving);
		}

		return Result.success;
	}
	// end of Sequence Node

	// 移动的Sequence Node
	Selector GetMoveSelector() {
		Sequence pit = GetPitSequence ();
		Sequence dog = GetDogSequence ();
		Sequence meat = GetMeatSequence ();
		Sequence moveToFire = GetMoveToCampFireSeq ();
		return new Selector (pit, dog, meat, moveToFire);
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
		agent.speed = 1.0f+ RandomSpeed();
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
		Action fall = new Action(Fall);

		return new Sequence (isFire, isPit, stop, fall);
	}


//	float scaleTime = 0.0f;
//	float fallTime = 1.0f;

	Result Fall() {

		float scalex = (float)(gameObject.transform.localScale.x);
		float scaley = (float)(gameObject.transform.localScale.y);

		int dir = scalex / Mathf.Abs (scalex);
		scalex = Mathf.Abs (scalex);
		scalex -= 0.001f;
		scaley -= 0.001f;

		scalex *= dir;
		gameObject.transform.localScale = new Vector3 (scalex, scaley, 1);
		Debug.Log (scalex);

		if (scalex > 0) {
			Debug.Log (scalex);
			return Result.running;
		}

		World.Instance.Remove (gameObject);
		return Result.success;
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
			animator.SetBool ("Ext", false);
			animator.SetBool ("Run", true);
			agent.speed = 5.0f + RandomSpeed();
			agent.SetDestination (new Vector2(nearestMeat.transform.position.x, nearestMeat.transform.position.y));
		}

		return Result.success;
	}

	bool IsArriveMeat() {
		return isHitMeat;
	}

	Result EatMeat() {
		//		Debug.Log ("out fire");
		attackTimer += Time.deltaTime;

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
			
		if (attackTimer < attackTime) {
			animator.SetBool ("Ext", true);

			return Result.running;
		}

		animator.SetBool ("Ext", false);
		if (nearestMeat) {
			World.Instance.Remove (nearestMeat);
		}
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

	Sequence GetDogSequence() {
		Condition isFire = new Condition (IsFire);

		Condition isInRange = new Condition (IsInRange);
		Action escape = new Action (Escape);

		Selector dog = new Selector(new Sequence(isInRange, escape));
		return new Sequence (isFire, dog);

	}

	bool IsInRange() {
		List<GameObject> objs = World.Instance.GetModels ();

		foreach (GameObject obj in objs) {
			if (obj.tag == "Dog") {
				float dis = Vector3.Distance (transform.position, obj.transform.position);

				if(dis < DOG_RANGE) {
					return true;
				}
			}
		}
		return false;
	}

	Result Escape() {
		List<GameObject> objs = World.Instance.GetModels ();

		float distance = 0;
		GameObject nearestDog = null;
		foreach (GameObject obj in objs) {
			if (obj.tag == "Dog") {
				float dis = Vector3.Distance (transform.position, obj.transform.position);
				if (distance == 0 || distance > dis) {
					distance = dis;
					nearestDog = obj;
				}
			}
		}

		if(nearestDog) {
//			Vector3 dir = (nearestDog.transform.position - transform.position).normalized;

			float x = 14;
			float y = Random.Range(0, 18) - 9;
			if (nearestDog.transform.position.x - transform.position.x > 0) {
				x = -13;

			} else {
				x = 13;
			}

			animator.SetBool ("Ext", false);
			animator.SetBool ("Run", true);

			agent.SetDestination (new Vector2(x, y));

			agent.speed = 5.0f + RandomSpeed();
			animator.SetBool ("Run", true);
		}

		return Result.success;
	}

	float RandomSpeed() {
		return (Random.Range (0, 10) - 5) / 10.0f;
	}
}
