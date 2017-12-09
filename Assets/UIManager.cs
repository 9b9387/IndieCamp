using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HandyEvent;

public class UIManager : MonoBehaviour {

	public static UIManager Instant;

	public ItemIcon icon;
	public GameObject[] img_icons;

	List<RectTransform> items;

	ItemIcon currentSelected;
	RectTransform shadow;

	void Start(){
		if (Instant == null){
			Instant = this;
		}else if (Instant != this){
			Destroy (gameObject);
		}

		items = new List<RectTransform> ();

		EventManager.Instance.AddListener (HandyEvent.EventType.click_item, OnClickItem);

		ItemTypes[] types = {ItemTypes.dog, ItemTypes.dog, ItemTypes.meet, ItemTypes.pit, ItemTypes.meet};
		InitItemBar (types);
	}

	void OnClickItem(EventArgs args){
		ItemIcon item = args.GetValue<ItemIcon> ();
		if (currentSelected == null){
			currentSelected = item;
			item.HideIcons ();
			shadow = item.Icon.GetComponent<RectTransform> ();
		}
	}

	void Update(){
		if (shadow){
//			Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			shadow.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
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
}
