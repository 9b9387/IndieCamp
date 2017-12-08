using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TestBehaviourTree : MonoBehaviour {

	public Vector3[] points;
	public LayerMask enemyMask;
	public float attackTime = 2f;

	Vector3 currentPoint;
	int currentIdx = 0;
	GameObject enemy;
	Selector root;

	float attackTimer = 0;

	void Start () {
		currentPoint = points [currentIdx];

		Condition isEnemyNeer = new Condition (IsEnemyNeer);
		Action attackEnemy = new Action (Attack);
		Sequence tryAttack = new Sequence (isEnemyNeer, attackEnemy);

		Action moveToEnemy = new Action (MoveToEnemy);
		Selector moveToAndAttackEnemy = new Selector (tryAttack, moveToEnemy);

		Condition hasEnemy = new Condition (HasEnemy);
		Sequence attack = new Sequence (hasEnemy, moveToAndAttackEnemy);

		Condition node4 = new Condition (IsTooFarToCurrPoint);
		Action node5 = new Action (MoveToCurPoint);
		Sequence patrol = new Sequence (node4, node5);

		root = new Selector (attack, patrol);
	}

	void Update () {
		root.Execute ();
	}

	bool HasEnemy(){
		RaycastHit[] hits = Physics.SphereCastAll (transform.position, 10, Vector3.forward, 0, enemyMask);
		if (hits.Length > 0){
			enemy = hits [0].collider.gameObject;
			return true;
		}

		enemy = null;
		return false;
	}

	bool IsEnemyNeer(){
		if (enemy){
			float dist = Vector3.Distance (enemy.transform.position, transform.position);
			return dist < 1.5f;
		}

		return false;
	}

	Result Attack(){
		attackTimer += Time.deltaTime;
		if (attackTimer < attackTime){
			print ("attacking.......");
			return Result.running;
		}

		if (enemy){
			enemy.SetActive (false);
			attackTimer = 0;
			print ("attack success!!!");
			return Result.success;
		}

		return Result.failure;

	}

	Result MoveToEnemy(){
		if (enemy){
			Vector3 dir = (enemy.transform.position - transform.position).normalized;
			transform.position += dir * 4 * Time.deltaTime;
		}

		return Result.success;
	}

	bool IsTooFarToCurrPoint(){
		float dist = Vector3.Distance (transform.position, currentPoint);
		if (dist > 0.5f){
			return true;
		}

		currentIdx++;
		if (currentIdx == points.Length){
			currentIdx = 0;
		}
		currentPoint = points [currentIdx];

		return false;
	}

	Result MoveToCurPoint(){
		Vector3 dir = (currentPoint - transform.position).normalized;
		transform.position += dir * 2 * Time.deltaTime;

		return Result.success;
	}
}
