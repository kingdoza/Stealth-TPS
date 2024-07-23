using System;
using UnityEngine;

public class DestroyableSingleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static object lockObject = new object();
    private static T instance;
    public static T Instance {
        get {
            if (instance == null) {
                lock (lockObject) {
                    if (instance == null) {
                        instance = FindObjectOfType<T>();
                        if (instance == null) {
                            GameObject gameObject = new GameObject(typeof(T).Name, typeof(T));
                            instance = gameObject.GetComponent<T>();
                        } else {
                            // 중복 검사 및 처리
                            T existingInstance = FindObjectOfType<T>();
                            if (existingInstance != null && existingInstance != instance) {
                                Destroy(existingInstance.gameObject);
                            }
                        }
                    }
                }
            }
            return instance;
        }
    }

    protected virtual void Awake() {
        if (instance == null) {
            instance = this as T;
        } else if (instance != this) {
            Destroy(gameObject); // Destroy duplicate
        }
    }

}