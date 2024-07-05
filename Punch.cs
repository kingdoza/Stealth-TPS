using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : Weapon {
    [SerializeField] private int damage = 1;
    [SerializeField] private float hitDelay = 2f;
    protected float remainedPunchDelay = 0;
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float attackAngle = 10f;

    private void Awake() {
    }

    private void Update() {
        remainedPunchDelay -= Time.deltaTime;
        if(remainedPunchDelay <= 0)
            remainedPunchDelay = 0f;
    }

    public bool TryStartHit() {
        if(remainedPunchDelay <= Mathf.Epsilon) {
            StartHit();
            return true;
        }
        return false;
    }

    public void HitPunch() {
        PlayerController player = null;
        if(IsTargetInFront(ref player)) {
            player.TakeHit(damage);
        }
    }

    private bool IsTargetInFront(ref PlayerController player) {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackDistance);
        foreach (Collider hitCollider in hitColliders) {
            if((player = hitCollider.GetComponent<PlayerController>()) == null)
                continue;
            Vector3 directionToPlayer = (hitCollider.transform.position - transform.position).normalized;
            float angleBetween = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleBetween < attackAngle) {
                return true;
            }
        }
        return false;
    }

    public void ResetHitDelay() {
        remainedPunchDelay = 0;
    }

    private void StartHit() {
        remainedPunchDelay = hitDelay;
    }
}
