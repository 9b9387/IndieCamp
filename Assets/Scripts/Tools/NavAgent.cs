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

	void Start () {
		EventManager.Instance.AddListener (HandyEvent.EventType.process_map_finish, OnProcessMapFinish);
	}

	public void SetDestination(Vector2 destination){
		m_currentIdx = 0;
		m_destination = destination;
		m_hasDestination = true;
		m_path = MapHandler.Instant.FindPath (new Vector2 (transform.position.x, transform.position.y), destination);
	}

	void OnProcessMapFinish(EventArgs args){
		if (m_hasDestination){
			m_currentIdx = 0;
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
			m_path = null;
			m_currentIdx = 0;
			m_hasDestination = false;
			return;
		}

		Vector2 currentPoint = m_path [m_currentIdx];
		Vector2 direction = (currentPoint - currentPos).normalized;
		Vector2 movement = direction * speed * Time.deltaTime;
		transform.position += new Vector3 (movement.x, movement.y, 0);

		bool isNeerToCurrentPoint = Vector2.Distance (new Vector2 (transform.position.x, transform.position.y), currentPoint) < 0.1f;
		if (isNeerToCurrentPoint){
			m_currentIdx++;
			if (m_currentIdx == m_path.Length){
				m_path = null;
				m_currentIdx = 0;
				m_hasDestination = false;
			}
		}
	}
}
