using UnityEngine;

public class PersistentSingleton<T> : DestroyableSingleton<T> where T : MonoBehaviour {
    private void Awake() {
        if(transform.root != null)
            DontDestroyOnLoad(transform.root.gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }
}