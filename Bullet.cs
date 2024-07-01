using System.Collections;
using UnityEngine;

public class Bullet : PooledObject {
    [SerializeField] private float flySpeed;

    public void FlyAhead() {
        StartCoroutine(KeepFlyingAhead());
    }

    private IEnumerator KeepFlyingAhead() {
        float flyDuration = 1000f;
        while(flyDuration > 0) {
            flyDuration -= Time.deltaTime;
            transform.position += -transform.forward * flySpeed * Time.deltaTime;
            yield return null;
        }
        Destroy();
    }
}
