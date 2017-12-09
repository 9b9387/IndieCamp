using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class World : Singleton<World> {

	public Vector2 spawnLocation;

	public GameObject huminidPrefabs;


	List<GameObject> models;
	// Use this for initialization
	void Start () {
		models = new List<GameObject> ();

		EventManager.Instance.AddListener (HandyEvent.EventType.nav_finished, OnEnterComplate);

		CreateHominid ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddModel(GameObject obj) {
		models.Add(obj);
	}

	public List<GameObject> GetModels() {
		return models;
	}

	public void CreateHominid() {
		GameObject huminid = Instantiate<GameObject> (huminidPrefabs, new Vector3(spawnLocation.x, spawnLocation.y, 0), Quaternion.identity);
		NavAgent agent = huminid.GetComponent<NavAgent> ();
		string tag = "enter";
		Vector2 pos = MapHandler.Instant.GetRandomPoint ();
		agent.SetDestination (pos, tag);

//
//		GameObject huminid2 = Instantiate<GameObject> (huminidPrefabs, new Vector3(spawnLocation.x, spawnLocation.y, 0), Quaternion.identity);
//		NavAgent agent2 = huminid2.GetComponent<NavAgent> ();
//		agent2.SetDestination (new Vector2(-2, 2), tag);
	}

	public void OnEnterComplate(EventArgs args) {
		FinishInfo info = args.GetValue<FinishInfo> ();
		string tag = info.tag;
		GameObject obj = info.obj;

		if (tag == "enter") {
			HominidAI ai = obj.GetComponent<HominidAI> ();
			ai.StartAI ();
		}
	}
}