using System;
using UnityEngine;

public class EnemyGun : Gun {
    protected override RaycastHit[] GetBulletRayHits() {
        Vector3 shootingDir = GetRandomDirectionWithAngle();
        shootRay = new Ray(muzzle.position, shootingDir);
        shootRay.origin = shootRay.origin - shootRay.direction * 0.2f;
        RaycastHit[] hits = Physics.RaycastAll(shootRay, gunRange, hittableMask);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        Debug.DrawRay(shootRay.origin, shootRay.direction * gunRange, Color.red, 1f);
        foreach (RaycastHit hit in hits) {
            if(hit.collider.GetComponent<Enemy>())
                continue;
            if(hit.collider.GetComponent<Player>() == null) {
                return null;
            }
            return new RaycastHit[]{hit};
        }
        return null;
    }

    protected override void HitTargets(RaycastHit[] hits, Ray ray) {
        foreach(RaycastHit hit in hits) {
            Agent agentHit = hit.collider.GetComponent<Player>();
            if(agentHit == null)
                return;
            ((Player)agentHit).TakeHit(ray.origin, damage);
        }
    }
}
