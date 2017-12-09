using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HandyEvent;

public class World : Singleton<World> {

	public Vector2 spawnLocation;

	public GameObject huminidPrefabs;


	List<GameObject> models;


	public AudioSource audio_bmg;
	public AudioSource audio_outfire;



	// Use this for initialization
	void Start () {
		models = new List<GameObject> ();

		EventManager.Instance.AddListener (HandyEvent.EventType.nav_finished, OnEnterComplate);
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_active, OnFireActive);
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_deactive, OnFireDeactive);
		EventManager.Instance.AddListener (HandyEvent.EventType.time_up, OnFireDeactive);
		EventManager.Instance.AddListener (HandyEvent.EventType.spawn_item, OnSpawnItem);


		CreateHominid ();
		OnFireDeactive (null);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddModel(GameObject obj) {
		models.Add(obj);
	}

	public void Remove(GameObject obj) {
//		foreach(GameObject o in models) {
//			if (o == obj) {
//			}
//		}
//
		models.Remove (obj);
		Destroy (obj);
	}

	public List<GameObject> GetModels() {
		return models;
	}

	public void OnSpawnItem(EventArgs args) {
		GameObject obj = args.GetValue<GameObject> ();

		AddModel (obj);
	}

	public void OnFireActive(EventArgs args) {
		audio_outfire.Play ();
		audio_bmg.Pause ();

		UIManager.Instant.StartTimer ();
	}

	public void OnFireDeactive(EventArgs args) {
		audio_bmg.Play ();
		audio_outfire.Stop ();
	}


	public void CreateHominid() {
		for (int i = 0; i < 5; i++) {
			GameObject huminid = Instantiate<GameObject> (huminidPrefabs, new Vector3 (spawnLocation.x, spawnLocation.y, 0), Quaternion.identity);
			NavAgent agent = huminid.GetComponent<NavAgent> ();
			string tag = "enter";
			Vector2 pos = MapHandler.Instant.GetRandomPoint ();
			agent.SetDestination (pos, tag);
			models.Add (huminid);
		}
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