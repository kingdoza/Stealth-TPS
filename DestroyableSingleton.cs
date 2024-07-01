using UnityEngine;

public class DestroyableSingleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T instance;
    public static T Instance {
        get {
            if(instance == null) {
                instance = (T)FindObjectOfType(typeof(T));
                if(instance == null) {
                    GameObject gameObject = new GameObject(typeof(T).Name, typeof(T));
                    instance = gameObject.GetComponent<T>();
                }
            }
            return instance;
        }
    }
}