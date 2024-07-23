using System;
using UnityEngine;

public class BulletHolePool : ObjectPool {
    public void MakeBulletHole(RaycastHit hit, Ray ray) {
        if(hit.collider.GetComponent<Agent>() != null)
            return;
        Vector3 bulletHolePos = hit.point + hit.normal * 0.01f;
        GameObject bulletHoleObject = PullOut(bulletHolePos, Quaternion.LookRotation(hit.normal));
        BulletHole bulletHole = bulletHoleObject.GetComponent<BulletHole>();
        bulletHole.HitOrigin = ray.origin;
        bulletHole.ShowNonPersonHitEffects();
        /*
        switch(agentHit) {
            case Enemy enemy :
                bulletHole.ShowPersonHitEffects();
                break;
            case null :
                bulletHole.ShowNonPersonHitEffects();
                break;
        } */
    }
}
