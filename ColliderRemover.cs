using UnityEngine;

public class ColliderRemover : MonoBehaviour {
    private void Start() {
        DisableAllColliders(transform);
    }

    private void DisableAllColliders(Transform parent) {
        Collider currentCollider = parent.GetComponent<Collider>();
        if (currentCollider != null) {
            currentCollider.enabled = false;
        }
        
        foreach (Transform child in parent) {
            DisableAllColliders(child);
        }
    }
}