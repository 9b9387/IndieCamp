using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HandyEvent;

public class FadeScreen : MonoBehaviour {

	public AnimationCurve curve;
	public Text dayText;
	public Text msg;

	public string[] msgs;
	public string[] days;

	Image fadeScreen;
	bool IsFading = false;

	void Start () {
		fadeScreen = GetComponent<Image> ();
		fadeScreen.color = new Color (0, 0, 0, 0);
		dayText.gameObject.SetActive (false);
		msg.gameObject.SetActive (false);
	}
	
	public void Fade(int day){
		if (IsFading)
			return;
		
		StartCoroutine (StartFade(day));
	}

	IEnumerator StartFade(int day){
		float time = 0f;
		IsFading = true;
		while(time < 2){
			time += Time.deltaTime;
			float cur = curve.Evaluate (time);
			fadeScreen.color = new Color (0, 0, 0, cur);
			yield return null;
		}

		IsFading = false;
		EventManager.Instance.PushEvent (HandyEvent.EventType.fade_finished, null);
		dayText.text = days [day - 1];
		msg.text = msgs [day - 1];
		dayText.gameObject.SetActive (true);
		msg.gameObject.SetActive (true);
		yield return new WaitForSeconds (2);
		EventManager.Instance.PushEvent (HandyEvent.EventType.start_new_day, day);
		fadeScreen.color = new Color (0, 0, 0, 0);
		dayText.gameObject.SetActive (false);
		msg.gameObject.SetActive (false);
	}
}
