using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HandyEvent;

public class World : Singleton<World> {
	public GameConfig[] config;

	public Vector2 spawnLocation;

	public GameObject huminidPrefabs;


	List<GameObject> models;

	public GameObject flame;


	public AudioSource audio_bmg;
	public AudioSource audio_outfire;


	GameConfig curCfg;
	int curDay;

	// Use this for initialization
	void Start () {
		models = new List<GameObject> ();

		EventManager.Instance.AddListener (HandyEvent.EventType.nav_finished, OnEnterComplate);
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_active, OnFireActive);
		EventManager.Instance.AddListener (HandyEvent.EventType.fire_deactive, OnFireDeactive);
		EventManager.Instance.AddListener (HandyEvent.EventType.time_up, OnFireDeactive);
		EventManager.Instance.AddListener (HandyEvent.EventType.spawn_item, OnSpawnItem);
//		EventManager.Instance.AddListener (HandyEvent.EventType.time_up, OnWin);
		EventManager.Instance.AddListener (HandyEvent.EventType.time_up, OnTimeUp);
//		EventManager.Instance.AddListener (HandyEvent.EventType.fade_finished, OnFadeFinish);
//		EventManager.Instance.AddListener (HandyEvent.EventType.start_new_day, OnStartNewDay);


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

	void OnTimeUp(EventArgs args) {
//		WoodFire obj = FindObjectOfType<WoodFire> ();
//		obj.gameObject.activeInHierarchy
		if (flame.activeInHierarchy) {
			CreateHominid ();
			EventManager.Instance.PushEvent (HandyEvent.EventType.fire_deactive, flame);
			flame.SetActive (false);
		} else {
			ClearItems ();
			RefreshItems (3);
		}
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

	public void OnWin(EventArgs args) {
		curDay++;
		ClearItems ();
//		ClearHuman ();
//		UIManager.Instant.Fade (curDay);
	}

	public void ClearHuman(){
		for (int i = 0; i < models.Count; i++) {
			GameObject obj = models [i];
			if (obj.tag == "Human"){
				Remove (obj);
			}
		}
	}

	public void OnFireDeactive(EventArgs args) {
		audio_bmg.Play ();
		audio_outfire.Stop ();
//
		ClearItems ();
		RefreshItems (3);
//		UIManager.Instant.InitItemBar (curCfg.items);
	}

	public void ClearItems() {
		for (int i = models.Count-1; i>=0; i--)
		{
			GameObject obj = models [i];
			if (obj.tag == "Dog" || obj.tag == "Meet" || obj.tag == "Pit") {
				Remove (obj);
			}
		}
	}

	public void RefreshItems(int count) {
		ItemTypes[] types = new ItemTypes[count];

		for (int i = 0; i < 3; i++) {
			int index = Random.Range (0, 3);
			Debug.Log ("idnex" + index);
			if(index == 1) {
				types[i] = ItemTypes.dog;
			}

			if(index == 2) {
				types[i] = ItemTypes.meet;
			}

			if(index == 0) {
				types[i] = ItemTypes.pit;
			}
		}
			
		UIManager.Instant.InitItemBar(types);
	}


	public void CreateHominid() {
//		for (int i = 0; i < 5; i++) {
			GameObject huminid = Instantiate<GameObject> (huminidPrefabs, new Vector3 (spawnLocation.x, spawnLocation.y, 0), Quaternion.identity);
			NavAgent agent = huminid.GetComponent<NavAgent> ();
			string tag = "enter";
			Vector2 pos = MapHandler.Instant.GetRandomPoint ();
			agent.SetDestination (pos, tag);
			models.Add (huminid);
//		}
//
//		GameObject huminid2 = Instantiate<GameObject> (huminidPrefabs, new Vector3(spawnLocation.x, spawnLocation.y, 0), Quaternion.identity);
//		NavAgent agent2 = huminid2.GetComponent<NavAgent> ();
//		agent2.SetDestination (new Vector2(-2, 2), tag);
	}

	public void CreateHominid(int count) {
		for (int i = 0; i < count; i++) {
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

//	public void StartGame(){
//		Debug.Log ("start");
//		UIManager.Instant.Fade (1);
//	}

//	void OnFadeFinish(EventArgs args){
//		Debug.Log ("fadefinish");
//		UIManager.Instant.HideTitle ();
//	}

	void OnStartNewDay(EventArgs args){
		int day = args.GetValue<int> ();
		GameConfig cfg = config [day - 1];
		curCfg = cfg;
		curDay = day;
		CreateHominid (cfg.manCount);
		UIManager.Instant.InitItemBar (cfg.items);
	}
}

[System.Serializable]
public class GameConfig{
	public int day;
	public int manCount;
	public ItemTypes[] items;
}