using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class NavAgent : MonoBehaviour {

	public float speed = 5;
	public float stoppingDistance = 1;

	Vector2[] m_path;
	Vector2 m_destination;
	bool m_hasDestination = false;
	int m_currentIdx = 0;
	bool m_isMoving = false;
	bool m_isArrive = true;
	string m_tag = "";

	public bool IsMoving{
		get{
			return m_isMoving;
		}
	}

	public bool IsArrive{
		get{
			return m_isArrive;
		}
	}

	void Start () {
		EventManager.Instance.AddListener (HandyEvent.EventType.process_map_finish, OnProcessMapFinish);
	}

	public void SetDestination(Vector2 destination){
		m_tag = "";
		m_currentIdx = 0;
		m_destination = destination;
		m_hasDestination = true;
		m_isArrive = false;
		m_path = MapHandler.Instant.FindPath (new Vector2 (transform.position.x, transform.position.y), destination);
	}

	public void SetDestination(Vector2 destination, string tag){
		m_tag = tag;
		m_currentIdx = 0;
		m_destination = destination;
		m_hasDestination = true;
		m_isArrive = false;
		m_path = MapHandler.Instant.FindPath (new Vector2 (transform.position.x, transform.position.y), destination);
	}

	void OnProcessMapFinish(EventArgs args){
		if (m_hasDestination){
			m_currentIdx = 0;
			m_isArrive = false;
			m_path = MapHandler.Instant.FindPath (new Vector2 (transform.position.x, transform.position.y), m_destination);
		}
	}

	void Update(){
		if (m_path == null)
			return;

		if (m_path.Length == 0)
			return;

		if (m_currentIdx >= m_path.Length)
			return;

		if (!m_hasDestination)
			return;
		
		Vector2 currentPos = new Vector2 (transform.position.x, transform.position.y);
		float distance = Vector2.Distance (currentPos, m_destination);

		if (distance < stoppingDistance) {
			m_isArrive = true;
			m_path = null;
			m_currentIdx = 0;
			m_hasDestination = false;
			m_isMoving = false;
			EventManager.Instance.PushEvent (HandyEvent.EventType.nav_finished, new FinishInfo(m_tag, gameObject));
			return;
		}

		Vector2 currentPoint = m_path [m_currentIdx];
		if (m_currentIdx == m_path.Length - 1){
			currentPoint = m_destination;
		}
		Vector2 direction = (currentPoint - currentPos).normalized;
		Vector2 movement = direction * speed * Time.deltaTime;
		transform.position += new Vector3 (movement.x, movement.y, 0);
		m_isMoving = true;
		m_isArrive = false;
//		int side = transform.position.x < m_destination.x ? -1 : 1;
//		transform.localScale = new Vector3 (side * transform.localScale.x, transform.localScale.y, transform.localScale.z);

		bool isNeerToCurrentPoint = Vector2.Distance (new Vector2 (transform.position.x, transform.position.y), currentPoint) < 0.1f;
		if (isNeerToCurrentPoint){
			m_currentIdx++;
			if (m_currentIdx == m_path.Length){
				m_isArrive = true;
				m_path = null;
				m_currentIdx = 0;
				m_hasDestination = false;
				m_isMoving = false;
				EventManager.Instance.PushEvent (HandyEvent.EventType.nav_finished, new FinishInfo(m_tag, gameObject));
			}
		}
	}
}

public class FinishInfo{
	public string tag;
	public GameObject obj;
	public FinishInfo(string _tag, GameObject _obj){
		tag = _tag;
		obj = _obj;
	}
}
