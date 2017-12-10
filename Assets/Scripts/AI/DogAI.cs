using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using HandyEvent;

public class DogAI : MonoBehaviour {

	public float DESTANCE = 3.0f;

	Selector root;
	NavAgent agent;
	Animator animator;


	bool isFire = false;
	bool isHitPit = false;
	bool isHitMeat = false;


	float attackTimer = 0;
	public float attackTime = 2f;
	float lastPosX = 0.0f;

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_active, OnFireActive);
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_deactive, OnFireDeactive);
		EventManager.Instance.AddListener (HandyEvent.EventType.hit_pit, OnHitPit);
		EventManager.Instance.AddListener (HandyEvent.EventType.hit_meet, OnHitMeat);

		agent = GetComponent<NavAgent> ();
		animator = GetComponent<Animator> ();

		animator.SetBool ("Run", false);


		Sequence pit = GetPitSequence ();
		Sequence meat = GetMeatSequence ();
		Sequence chase = GetChaseSquence ();
		Action stop = new Action (Stop);

		root = new Selector (meat, chase, stop);
	}

	void OnDestroy() {
		Debug.Log ("OnDestroy");
		EventManager.Instance.RemoveListener (HandyEvent.EventType.fire_active, OnFireActive);
		EventManager.Instance.RemoveListener (HandyEvent.EventType.fire_deactive, OnFireDeactive);
		EventManager.Instance.RemoveListener (HandyEvent.EventType.hit_pit, OnHitPit);
		EventManager.Instance.RemoveListener (HandyEvent.EventType.hit_meet, OnHitMeat);
	}
	
	// Update is called once per frame
	void Update () {
		root.Execute ();

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

	void OnHitPit(EventArgs args) {
		GameObject obj = args.GetValue<GameObject> ();

		if (obj == gameObject && isFire) {
			isHitPit = true;
		}
	}

	void OnHitMeat(EventArgs args) {
		GameObject obj = args.GetValue<GameObject> ();

		if (obj == gameObject && isFire) {
			isHitMeat = true;
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
			animator.SetBool ("Run", true);
			agent.SetDestination (new Vector2(nearestHuman.transform.position.x, nearestHuman.transform.position.y));

		}
		
		return Result.success;
	}


	bool IsHitPit() {
		return isHitPit;
	}

	Result Stop() {
		agent.StopMove ();
		animator.SetBool ("Run", false);
		return Result.success;
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
//				Debug.Log(dis + " " + MEAT_RANGE + " " + (dis < MEAT_RANGE));
				if(dis != 0 && dis < DESTANCE) {
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
		Debug.Log("isHitMeat " + isHitMeat);
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
			animator.SetBool ("Run", false);
			return Result.running;
		}

//		animator.SetBool ("Ext", false);
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
}
