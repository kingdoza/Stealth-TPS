using System.Collections.Generic;
using UnityEngine;

public class HitMarkController : ObjectPool {
    public void GenerateHitMark(Vector3 hitOrigin) {
        HitMark hitmark = PullOut(transform.position).GetComponent<HitMark>();
        hitmark.LookOriginDirection(hitOrigin);
        hitmark.HitOrigin = hitOrigin;
    }
}
