using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HandyEvent;

public class UIManager : MonoBehaviour {

	public static UIManager Instant;

	public ItemIcon icon;
	public GameObject[] img_icons;
	public GameObject[] itemObjects;
	public Sprite[] nums;
	public Image timer;
	public FadeScreen fadeScreen;

	List<RectTransform> items;

	ItemIcon currentSelected;
	GameObject shadow;
	bool isTimerRunning = false;

	void Start(){
		if (Instant == null){
			Instant = this;
		}else if (Instant != this){
			Destroy (gameObject);
		}

		items = new List<RectTransform> ();

		EventManager.Instance.AddListener (HandyEvent.EventType.click_item, OnClickItem);
		EventManager.Instance.AddListener (HandyEvent.EventType.player_action, OnPlayerAction);

//		ItemTypes[] types = {ItemTypes.dog, ItemTypes.dog, ItemTypes.meet, ItemTypes.pit, ItemTypes.meet};
//		InitItemBar (types);

//		StartTimer ();
	}

	void OnClickItem(EventArgs args){
		ItemIcon item = args.GetValue<ItemIcon> ();
		if (currentSelected == null){
			currentSelected = item;
			item.HideIcons ();
			shadow = Instantiate<GameObject> (img_icons[(int)item.Type]);
		}
	}

	void OnPlayerAction(EventArgs args){
		StandardInputType type = args.GetValue<StandardInputType> ();
		if (type == StandardInputType.mouse_left_button_down){
			if (currentSelected){
				Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				if (itemObjects[(int)currentSelected.Type]){
					GameObject obj = Instantiate (itemObjects[(int)currentSelected.Type]) as GameObject;
					obj.transform.position = pos;
					obj.GetComponent<SpriteRenderer> ().sortingOrder = 1;
					currentSelected = null;
					Destroy (shadow);
					shadow = null;
					EventManager.Instance.PushEvent (HandyEvent.EventType.spawn_item, obj);
				}
			}
		}

		if (type == StandardInputType.mouse_right_button_down){
			if (currentSelected){
				currentSelected.Init (currentSelected.Type);
				currentSelected = null;
				Destroy (shadow);
				shadow = null;
			}
		}
	}

	void Update(){
		if (shadow){
			Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			shadow.transform.position = pos;
		}
	}

	IEnumerator Timer(){
		int cur = 3;
		isTimerRunning = true;
		while(cur > 0){
			cur--;
			timer.gameObject.SetActive (true);
			timer.sprite = nums [cur];
			yield return new WaitForSeconds (1);
		}

		timer.gameObject.SetActive (false);
		isTimerRunning = false;
		EventManager.Instance.PushEvent (HandyEvent.EventType.time_up, null);
	}

	public void StartTimer(){
		if (!isTimerRunning){
			StartCoroutine (Timer ());
		}
	}

	public void InitItemBar(ItemTypes[] types){
		foreach (var item in items) {
			Destroy (item.gameObject);
		}
		items.Clear ();

		for (int i = 0; i < types.Length; i++) {
			ItemIcon item = Instantiate<ItemIcon> (icon) as ItemIcon;
			item.Init (types[i]);
			RectTransform tran = item.GetComponent<RectTransform> ();
			tran.SetParent (transform);
			float posX = i * 100;
			tran.anchoredPosition = new Vector2 (posX, 0);
			tran.anchorMax = Vector2.zero;
			tran.anchorMin = Vector2.zero;
			items.Add (tran);
		}
	}

	public void Fade(){
		fadeScreen.Fade ();	
	}
}
