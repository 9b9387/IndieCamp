using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStar;
using HandyEvent;

[RequireComponent(typeof(BoxCollider2D))]
public class MapHandler : MonoBehaviour {

	static MapHandler _instant;
	public static MapHandler Instant{
		get{
			return _instant;
		}
	}

	public LayerMask blockMask;
	public int resolutionX = 16;
	public int resolutionY = 9;

	float m_sizeX;
	float m_sizeY;
	float m_unitSizeX;
	float m_unitSizeY;

	PathFinder m_pathFinder;
	MapData m_mapData;
	BoxCollider2D m_coll;
	Vector2 m_origin;
	Vector2 m_center;

	void Start () {
		if (_instant == null){
			_instant = this;
		}else if (_instant != this){
			Destroy (gameObject);
		}

		m_center = transform.position;
		m_coll = GetComponent<BoxCollider2D> ();
		m_sizeX = transform.localScale.x * m_coll.size.x;
		m_sizeY = transform.localScale.y * m_coll.size.y;
		m_origin = new Vector2 (m_center.x - m_sizeX / 2, m_center.y - m_sizeY / 2);

		m_unitSizeX = m_sizeX / resolutionX;
		m_unitSizeY = m_sizeY / resolutionY;

		m_mapData = new MapData ();
		m_mapData.mapWidth = resolutionX;
		m_mapData.mapHeight = resolutionY;
		m_mapData.positions = new Vector2[resolutionX * resolutionY];
		m_mapData.tileTypes = new TileType[resolutionX * resolutionY];

		ProcessMap ();
	}

	public void ProcessMap(){
		for (int y = 0; y < resolutionY; y++) {
			for (int x = 0; x < resolutionX; x++) {
				float posX = m_center.x - m_sizeX / 2 + m_unitSizeX * x + m_unitSizeX / 2;
				float poxY = m_center.y - m_sizeY / 2 + m_unitSizeY * y + m_unitSizeY / 2;
				int idx = x + y * resolutionX;
				Vector2 pos = new Vector2 (posX, poxY);
				m_mapData.positions [idx] = pos;

				Collider2D coll = Physics2D.OverlapPoint (pos, blockMask);
				m_mapData.tileTypes [idx] = coll == null ? TileType.Empty : TileType.Block;
			}
		}

		m_pathFinder = new PathFinder (m_mapData);

		EventManager.Instance.PushEvent (HandyEvent.EventType.process_map_finish, null);
	}

	public Vector2[] FindPath(Vector2 startPos, Vector2 endPos){
		int startIdx = WorldPosToIndex (startPos);
		int endIdx = WorldPosToIndex (endPos);
		if (startIdx == -1 || endIdx == -1){
			return null;
		}

		List<int> pathIdxes = m_pathFinder.FindPath (startIdx, endIdx);
		if (pathIdxes == null){
			return null;
		}

		Vector2[] path = new Vector2[pathIdxes.Count];
		for (int i = 0; i < pathIdxes.Count; i++) {
			path [i] = m_mapData.positions [pathIdxes [i]];
		}

		return path;
	}

	public Vector2 GetRandomPoint(){
		while (true) {
			float x = Random.Range (Screen.width / 4, Screen.width / 4 * 3);
			float y = Random.Range (Screen.height / 4, Screen.height / 4 * 3);

			Vector2 pos = Camera.main.ScreenToWorldPoint (new Vector3 (x, y, 0));
			int idx = WorldPosToIndex (pos);
			if (idx == -1)
				return Vector2.zero;
			
			if (m_mapData.tileTypes[idx] == TileType.Empty){
				return pos;
			}
		}
	}

	public Vector2 GetRandomPoint(bool isFarDis){
		while (true) {
			float x = Random.Range (Screen.width / 4, Screen.width / 4 * 3);
			float y = Random.Range (Screen.height / 4, Screen.height / 4 * 3);
			if (isFarDis){
				x = Random.Range (0, 1) > 0.5f ? Random.Range (0, Screen.width / 4) : Random.Range (Screen.width / 4 * 3, Screen.width);
				y = Random.Range (0, 1) > 0.5f ? Random.Range (0, Screen.height / 4) : Random.Range (Screen.height / 4 * 3, Screen.height);
			}
			Vector2 pos = Camera.main.ScreenToWorldPoint (new Vector3 (x, y, 0));
			int idx = WorldPosToIndex (pos);
			if (idx == -1)
				return Vector2.zero;

			if (m_mapData.tileTypes[idx] == TileType.Empty){
				return pos;
			}
		}
	}

	int WorldPosToIndex(Vector2 pos){
		float minX = transform.position.x - m_sizeX / 2;
		float maxX = transform.position.x + m_sizeX / 2;
		bool isInRangeX = pos.x >= minX && pos.x <= maxX;

		float minY = transform.position.y - m_sizeY / 2;
		float maxY = transform.position.y + m_sizeY / 2;
		bool isInRangeY = pos.y >= minY && pos.y <= maxY;

		if (!isInRangeX || !isInRangeY){
			return -1;
		}

		float disX = pos.x - m_origin.x;
		float disY = pos.y - m_origin.y;

		int gridX = Mathf.FloorToInt (disX / m_unitSizeX);
		int gridY = Mathf.FloorToInt (disY / m_unitSizeY);

		return gridX + gridY * m_mapData.mapWidth;
	}

	void OnDrawGizmos(){
		if (m_mapData == null)
			return;
		
		for (int i = 0; i < m_mapData.tileTypes.Length; i++) {
			Gizmos.color = m_mapData.tileTypes [i] == TileType.Empty ? Color.blue : Color.red;
			Gizmos.DrawCube (new Vector3(m_mapData.positions[i].x, m_mapData.positions[i].y, 0), new Vector3(m_unitSizeX - 0.05f, m_unitSizeY - 0.05f, 1));
		}
	}
}
