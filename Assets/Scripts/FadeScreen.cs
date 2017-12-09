using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HandyEvent;

public class FadeScreen : MonoBehaviour {

	public AnimationCurve curve;

	Image fadeScreen;
	bool IsFading = false;

	void Start () {
		fadeScreen = GetComponent<Image> ();
		fadeScreen.color = new Color (0, 0, 0, 0);
		Fade ();
	}
	
	public void Fade(){
		if (IsFading)
			return;
		
		StartCoroutine (StartFade());
	}

	IEnumerator StartFade(){
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
		yield break;
	}
}
