using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmedEnemy : Enemy {
    private Gun gun;

    protected override void Awake() {
        base.Awake();
        gun = GetComponentInChildren<EnemyGun>();
    }

    protected override void Start() {
        base.Start();
        transform.GetChild(1).GetComponent<Renderer>().material.color = Color.white;
    }

    protected override void SetAlertSequence() {
        alertSequence = new Sequence(new List<BT_Node> {
            //new ConditionNode(() => currentReaction == EnemyReaction.Alert),
            new OnceNode(new ActionNode(SetMoveStateToStop)),
            new OnceNode(new ActionNode(SetPlayerPos)),
            new OnceNode(new ActionNode(TurnRapidly)),
            new ActionNode(SetChaseSpeed),
            new Parallel(new List<BT_Node> {
                new ActionNode(ChasePlayer),
                new ActionNode(ShotPlayer)
            })
        });
    }

    private BT_NodeState ShotPlayer() {
        Debug.Log("Shot Player");
        bool isTargetInGunRange = IsTargetInGunRange(Player.Instance);
        if(!isTargetInGunRange || movingState == MovingState.Running)
            return BT_NodeState.Failure;
        gun.PullTrigger();
        return BT_NodeState.Success;
    }

    protected override BT_NodeState SetChaseSpeed() {
        float distanceToTarget = Vector3.Distance(transform.position, Player.Instance.Position);
        if(distanceToTarget >= 20) {
            MovingState = MovingState.Running;
        }
        else if(distanceToTarget >= 5 || !IsTargetInGunRange(Player.Instance)) {
            MovingState = MovingState.Crouching;
        }
        else {
            MovingState = MovingState.Stop;
        }
        return BT_NodeState.Success;
    }






    protected override void Alert(IEnemyReactable reactable) {
        //Debug.Log(reactable);
        StopCoroutine(approachingProcess);
        StartCoroutine(DoAlertProcedures(reactable));
    }

    private IEnumerator DoAlertProcedures(IEnemyReactable reactable) {
        yield return StartCoroutine(KeepTurningTo(reactable.Position, 10f));
        StartCoroutine(StartApproachingTo(reactable.Transform, MovingState.Crouching));
        bool isTargetInGunRangePre = false;
        IEnumerator shootingProcess = KeepShootingTarget(reactable.Transform);
        while(currentReaction == EnemyReaction.Alert) {
            yield return null;
            AdjustAlertMoveSpeedTo(reactable);
            TryShoot(reactable, ref isTargetInGunRangePre, ref shootingProcess);
        }
    }

    private void TryShoot(IEnemyReactable reactable, ref bool isTargetInGunRangePre, ref IEnumerator shootingProcess) {
        bool isTargetInGunRangeCur = IsTargetInGunRange(reactable);
        if(isTargetInGunRangeCur == isTargetInGunRangePre)
            return;
        isTargetInGunRangePre = isTargetInGunRangeCur;
        if(isTargetInGunRangeCur) {
            StartCoroutine(shootingProcess);
        }
        else {
            StopCoroutine(shootingProcess);
        }
    }

    protected override void AdjustAlertMoveSpeedTo(IEnemyReactable reactable) {
        float distanceToTarget = Vector3.Distance(transform.position, reactable.Transform.position);
        if(distanceToTarget >= 20) {
            movingState = MovingState.Running;
        }
        else if(distanceToTarget >= 5) {
            movingState = MovingState.Crouching;
        }
        else {
            movingState = MovingState.Stop;
        }
    }

    private bool IsTargetInGunRange(IEnemyReactable reactable) {
        float distanceToTarget = Vector3.Distance(transform.position, reactable.Transform.position);
        //Debug.Log(IsVisibleFromEyesight(reactable) && distanceToTarget <= 30);
        return IsVisibleFromEyesight(reactable) && gun.IsValidShotRay(reactable);
    }

    private IEnumerator KeepShootingTarget(Transform target) {
        float shootingDelay = 0.5f;
        while(target) {
            LookAt(target.position);
            shootingDelay -= Time.deltaTime;
            if(shootingDelay <= 0) {
                gun.PullTrigger();
                shootingDelay = 0.5f;
            }
            yield return null;
        }
    }
}
