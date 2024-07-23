using UnityEngine;

public static class Utility {
    public static string ToPercentage(this int number) {
        int percentage = Mathf.Clamp(number * 100, 0, 100);
        return percentage + "%";
    }

    public static string ToPercentage(this float number) {
        int percentage = Mathf.Clamp((int)number * 100, 0, 100);
        return percentage + "%";
    }

    public static T FindNonChildObjectOfType<T>() where T : MonoBehaviour {
        T[] objects = GameObject.FindObjectsOfType<T>();
        foreach (T obj in objects) {
            if (obj.transform.parent == null) {
                return obj;
            }
        }
        return null;
    }
}
