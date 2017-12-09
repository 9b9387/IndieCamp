using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HandyEvent;

public enum ItemTypes{
	meet,
	dog,
	pit,
}

public class ItemIcon : MonoBehaviour, IPointerClickHandler{

	public Image[] icons;
	public ItemTypes Type{
		get{
			return type;
		}
	}

	public Image Icon{
		get{
			return icon;
		}
	}

	ItemTypes type;
	Image icon;

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData){
		EventManager.Instance.PushEvent (HandyEvent.EventType.click_item, this);
	}
	#endregion

	public void Init(ItemTypes _type){
		type = _type;

		HideIcons ();

		switch(type){
		case ItemTypes.meet:
			icons [0].gameObject.SetActive (true);
			icon = icons [0];
			break;
		case ItemTypes.dog:
			icons [1].gameObject.SetActive (true);
			icon = icons [1];
			break;
		case ItemTypes.pit:
			icons [2].gameObject.SetActive (true);
			icon = icons [2];
			break;
		default:
			break;
		}
	}

	public void HideIcons(){
		foreach (var icon in icons) {
			icon.gameObject.SetActive (false);
		}
	}
}
