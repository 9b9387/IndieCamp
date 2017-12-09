using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandyEvent;

public class StandardInput : Singleton<StandardInput> {

	public bool enableFixedUpdate = false;
	public bool enableClickObject3D = false;
	public bool enableClickObject2D = false;
	public LayerMask clickableMask;

	Camera m_cam;

	void Start () {
		StartCoroutine (HandleInput ());

		GameObject camObj = GameObject.FindGameObjectWithTag ("MainCamera");
		if (camObj == null){
			Debug.LogError ("Cannot find the object taged with 'MainCamera'.");
			return;
		}else{
			m_cam = camObj.GetComponent<Camera> ();
		}

		if (m_cam == null){
			Debug.LogError ("The camera object taged with 'MainCamera' doesn't contain 'Camera' component.");
		}
	}

	protected override void OnApplicationQuit(){
		base.OnApplicationQuit ();
		StopCoroutine (HandleInput ());
	}

	IEnumerator HandleInput () {
		while (true) {
			float x = Input.GetAxis ("Horizontal");
			float y = Input.GetAxis ("Vertical");
			if ((x != 0 || y != 0) && !enableFixedUpdate) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_axis, new StandardInputAxis (StandardInputType.keyboard_axis, new Vector2 (x, y)));
			}

			float mX = Input.GetAxis ("Mouse X");
			float mY = Input.GetAxis ("Mouse Y");
			if ((mX != 0 || mY != 0) && !enableFixedUpdate) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_axis, new StandardInputAxis (StandardInputType.mouse_axis, new Vector2 (mX, mY)));
			}
				
			float mouseScrollDelta = Input.GetAxis ("Mouse ScrollWheel");
			if (mouseScrollDelta != 0) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_axis, new StandardInputAxis(StandardInputType.mouse_scrollWheel_axis, new Vector2(mouseScrollDelta, 0)));
			}

			if (Input.GetMouseButtonDown (0)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.mouse_left_button_down);

				if (enableClickObject3D){
					HandleClickObject (StandardInputType.mouse_left_button_down, true);
				}

				if (enableClickObject2D){
					HandleClickObject (StandardInputType.mouse_left_button_down, false);
				}
			}

			if (Input.GetMouseButton (0)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.mouse_left_button_hold);
			}

			if (Input.GetMouseButtonUp (0)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.mouse_left_button_up);
			}

			if (Input.GetMouseButtonDown (1)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.mouse_right_button_down);
			}

			if (Input.GetMouseButton (1)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.mouse_right_button_hold);
			}

			if (Input.GetMouseButtonUp (1)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.mouse_right_button_up);
			}

			if (Input.GetMouseButtonDown (2)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.mouse_scrollWheel_down);
			}

			if (Input.GetMouseButton (2)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.mouse_scrollWheel_hold);
			}

			if (Input.GetMouseButtonUp (2)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.mouse_scrollWheel_up);
			}

			if (Input.GetKeyDown (KeyCode.Q)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.keyboard_Q);
			}

			if (Input.GetKeyDown (KeyCode.E)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.keyboard_E);
			}

			if (Input.GetKeyDown (KeyCode.R)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.keyboard_R);
			}

			if (Input.GetKeyDown (KeyCode.J)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.keyboard_J);
			}

			if (Input.GetKeyDown (KeyCode.K)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.keyboard_K);
			}

			if (Input.GetKeyDown (KeyCode.L)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.keyboard_L);
			}

			if (Input.GetKeyDown (KeyCode.U)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.keyboard_U);
			}

			if (Input.GetKeyDown (KeyCode.I)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.keyboard_I);
			}

			if (Input.GetKeyDown (KeyCode.O)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.keyboard_O);
			}

			if (Input.GetKeyDown (KeyCode.Space)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.keyboard_Space);
			}

			if (Input.GetKeyDown (KeyCode.Escape)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.player_action, StandardInputType.keyboard_Ese);
			}

			yield return null;
		}
	}

	void FixedUpdate(){
		if (!enableFixedUpdate)
			return;
		
		float x = Input.GetAxis ("Horizontal");
		float y = Input.GetAxis ("Vertical");
		if (x != 0 || y != 0) {
			EventManager.Instance.PushEvent (HandyEvent.EventType.player_axis, new StandardInputAxis (StandardInputType.keyboard_axis, new Vector2 (x, y)));
		}

		float mX = Input.GetAxis ("Mouse X");
		float mY = Input.GetAxis ("Mouse Y");
		if (mX != 0 || mY != 0) {
			EventManager.Instance.PushEvent (HandyEvent.EventType.player_axis, new StandardInputAxis (StandardInputType.mouse_axis, new Vector2 (mX, mY)));
		}
	}

	void HandleClickObject(StandardInputType type, bool is3dObject){
		if (is3dObject) {
			Ray ray = m_cam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000.0f, clickableMask)) {
				EventManager.Instance.PushEvent (HandyEvent.EventType.mouse_click_object, new ClickInfo (type, hit.collider.gameObject));
			}
		}
	
		RaycastHit2D hit2d = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, clickableMask);
		if(hit2d.collider != null){
			EventManager.Instance.PushEvent (HandyEvent.EventType.mouse_click_object, new ClickInfo(type, hit2d.collider.gameObject));
		}
	}
}

public class StandardInputAxis{
	StandardInputType type;
	Vector2 axis;

	public StandardInputType Type{
		get{
			return type;
		}
	}

	public Vector2 Axis{
		get{
			return axis;
		}
	}

	public StandardInputAxis(StandardInputType _type, Vector2 _axis){
		type = _type;
		axis = _axis;
	}
}

public class ClickInfo{
	StandardInputType type;
	GameObject obj;

	public StandardInputType Type{
		get{
			return type;
		}
	}

	public GameObject ClickedObject{
		get{
			return obj;
		}
	}

	public ClickInfo(StandardInputType _type, GameObject _obj){
		type = _type;
		obj = _obj;
	}
}

public enum StandardInputType{
	mouse_axis,
	mouse_scrollWheel_axis,
	mouse_left_button_down,
	mouse_left_button_hold,
	mouse_left_button_up,
	mouse_right_button_down,
	mouse_right_button_hold,
	mouse_right_button_up,
	mouse_scrollWheel_down,
	mouse_scrollWheel_hold,
	mouse_scrollWheel_up,

	keyboard_axis,
	keyboard_Q,
	keyboard_E,
	keyboard_R,
	keyboard_J,
	keyboard_K,
	keyboard_L,
	keyboard_U,
	keyboard_I,
	keyboard_O,
	keyboard_Space,
	keyboard_Ese,

}
