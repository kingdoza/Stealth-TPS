using UnityEngine;

public class BulletHolePool : ObjectPool {
    public void MakeBulletHole(RaycastHit hit, Ray ray) {
        Vector3 bulletHolePos = hit.point + hit.normal * 0.01f;
        GameObject bulletHoleObject = PullOut(bulletHolePos, Quaternion.LookRotation(hit.normal));
        BulletHole bulletHole = bulletHoleObject.GetComponent<BulletHole>();
        bulletHole.HitOrigin = ray.origin;
        Agent agentHit = hit.collider.GetComponent<Agent>();
        if(agentHit == null) {
            bulletHole.ShowNonPersonHitEffects();
        } else {
            agentHit.Die();
            bulletHole.ShowPersonHitEffects();
        }
    }
}
