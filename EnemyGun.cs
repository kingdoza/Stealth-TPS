using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : Gun {
    protected override RaycastHit[] GetBulletRayHits(out Ray ray) {
        Vector3 shootingDir = GetRandomDirectionWithAngle();
        ray = new Ray(transform.position, shootingDir);
        RaycastHit[] hits = Physics.RaycastAll(ray, gunRange, hittableMask);
        Debug.DrawRay(transform.position, shootingDir * gunRange, Color.red, 1f);
        foreach (RaycastHit hit in hits) {
            if(hit.collider.GetComponent<PlayerController>() != null) {
                return hits;
            }
        }
        return hits;
    }
}
