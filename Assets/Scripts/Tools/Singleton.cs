using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour{

	private static T _instance;
	private static readonly object _lock = new object();
	private static bool isQuiting = false;

	public static T Instance{
		get{
			lock(_lock){
				if (isQuiting){
					return default(T);
				}

				if (_instance == null){
					GameObject obj = new GameObject ("Singleton");
					_instance = obj.AddComponent <T> ();
				}

				return _instance;
			}
		}
	}

	protected virtual void Awake(){
		if (_instance == null) {
			T[] objs = GameObject.FindObjectsOfType <T>();
			if (objs.Length > 0)
				_instance = objs [0];

			if (objs.Length > 1){
				for (int i = 1; i < objs.Length; i++) {
					Destroy (objs[i].gameObject);
				}
			}
		}
	}

	protected virtual void OnApplicationQuit(){
		isQuiting = true;
	}
}
