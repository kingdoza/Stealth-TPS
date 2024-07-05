using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

internal enum WeaponState {
    Gun, Punch
}
public class Enemy : Agent {
    [SerializeField] private float viewRadius;
    [SerializeField] [Range(0, 360)] private float viewAngle;
    [SerializeField] private LayerMask obstacleMask;
    public Vector3 Position => transform.position;
    private bool isAlive;
    private EnemyReaction currentReaction;
    public EnemyReaction Reaction => currentReaction;
    private NavMeshAgent navMesh;
    private Vector3 moveTargetPos;
    private IEnumerator approachingProcess;
    private Vector3 previousPos;
    public Transform Head => head;
    [SerializeField] private LayerMask soundMask;
    [SerializeField] private int importance = 50;
    public int Importance => importance;
    [SerializeField] private int feeling = 50; //0~49 50 51~100
    public int Feeling => feeling;
    private Punch punch;
    private WeaponState weaponState = WeaponState.Punch;

    protected override void Awake() {
        punch = GetComponent<Punch>();
        base.Awake();
        SetFeelingColor();
        gun = GetComponentInChildren<EnemyGun>();
    }

    protected override void Start() {
        //SetWeaponToPunch();
        SetWeaponToGun();
        base.Start();
        isAlive = true;
        previousPos = transform.position;
        approachingProcess = ApproachingTo(transform.position);
        navMesh = GetComponent<NavMeshAgent>();
        currentReaction = EnemyReaction.None;
        StartCoroutine(KeepWatchingForward());

        MoveAround();
    }

    protected override void Update() {
        base.Update();
        CalculateMovingDir();
        //Debug.Log(movingState);
    }

    private void CalculateMovingDir() {
        Vector3 currentPos = transform.position;
        Vector3 posGap = currentPos - previousPos;
        Vector3 localPosGap = transform.InverseTransformDirection(posGap);
        movingDelta = localPosGap;
        previousPos = currentPos;
    }

    private IEnumerator KeepWatchingForward() {
        while(isAlive) {
            List<IEnemyReactable> reactables = GetEnemyReactablesInRange();
            //reactables.RemoveAll(element => element.EnemyReaction == EnemyReaction.None);
            //reactables = reactables.Where(IsVisibleFromEyesight).ToList();
            FilterInvisibleReactables(ref reactables);
            reactables = reactables.OrderBy(obj => obj.EnemyReaction).ToList();
            //foreach(var reactable in reactables) {
                //Debug.Log(reactable);
            //}
            if(reactables.Count() > 0) {
                TryChangeReaction(reactables[0]);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void TryChangeReaction(IEnemyReactable reactable) {
        if(reactable.EnemyReaction < currentReaction) {
            StopCoroutine(approachingProcess);
            ExecuteReaction(reactable);
            Debug.Log(currentReaction);
        }
    }

    private void ExecuteReaction(IEnemyReactable reactable) {
        currentReaction = reactable.EnemyReaction;
        Action<IEnemyReactable> action = currentReaction switch {
            EnemyReaction.Alert => Alert,
            EnemyReaction.Search => Search,
            EnemyReaction.Doubt => Doubt,
            _ => _ => { }
        };
        action(reactable);
    }

    private void Alert(IEnemyReactable reactable) {
        Debug.Log(reactable);
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
            if(weaponState == WeaponState.Gun) {
                //TryShoot(reactable, ref isTargetInGunRangePre, ref shootingProcess);
            }
            else {
                //TryPunch(reactable);
            }
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

    private void TryPunch(IEnemyReactable reactable) {
        float distance = Vector3.Distance(reactable.Position, transform.position);
        //agentAnimator.PlayPunchingPrepareAnim();
        if(punch.TryStartHit()) {
            agentAnimator.PlayPunchingAnim();
        }
    }

    private void SetWeaponToGun() {
        weaponState = WeaponState.Gun;
    }

    private void SetWeaponToPunch() {
        weaponState = WeaponState.Punch;
    }

    private void AdjustAlertMoveSpeedTo(IEnemyReactable reactable) {
        float distanceToTarget = Vector3.Distance(transform.position, reactable.Transform.position);
        if(weaponState == WeaponState.Punch) {
            if(distanceToTarget >= 1.2f) {
                movingState = MovingState.Running;
                punch.ResetHitDelay();
            }
            else {
                movingState = MovingState.Stop;
                TryPunch(reactable);
            }
            return;
        }

        if(distanceToTarget >= 20) {
            movingState = MovingState.Running;
        }
        else {
            movingState = MovingState.Crouching;
        }
    }

    private bool IsTargetInGunRange(IEnemyReactable reactable) {
        float distanceToTarget = Vector3.Distance(transform.position, reactable.Transform.position);
        return IsVisibleFromEyesight(reactable) && distanceToTarget <= 30;
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

    private void Search(IEnemyReactable reactable) {
        Debug.Log(reactable);
        movingState = MovingState.Walking;
        StartCoroutine(DoSearchProcedures(reactable));
        //근원지로 바로 바라봄
        //근원지로 중간 속도로 이동
    }

    private IEnumerator DoSearchProcedures(IEnemyReactable reactable) {
        navMesh.SetDestination(transform.position);
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(KeepTurningTo(reactable.HitOrigin, 10f));
        yield return new WaitForSeconds(1f);
        approachingProcess = ApproachingTo(reactable.HitOrigin);
        yield return StartCoroutine(approachingProcess);
        currentReaction = EnemyReaction.None;
        MoveAround();
    }

    private void Doubt(IEnemyReactable reactable) {
        Debug.Log(reactable);
        movingState = MovingState.Crouching;
        StartCoroutine(DoDoubtProcedures(reactable));
        //물체로 천천히 이동
        //근원지로 느린 속도로 이동
    }

    private void DetectDoubtfulPlace(Vector3 targetPos) {
        if(currentReaction < EnemyReaction.Doubt)
            return;
        movingState = MovingState.Crouching;
        currentReaction = EnemyReaction.Doubt;
        StopCoroutine(approachingProcess);
        StartCoroutine(DoDoubtProcedures(targetPos));
    }

    private IEnumerator DoDoubtProcedures(IEnemyReactable reactable) {
        navMesh.SetDestination(transform.position);
        yield return new WaitForSeconds(0.5f);
        approachingProcess = ApproachingTo(reactable.Position);
        yield return StartCoroutine(approachingProcess);
        yield return new WaitForSeconds(1f);
        if(reactable.HitOrigin != reactable.Position) {
            movingState = MovingState.Crouching;
            approachingProcess = ApproachingTo(reactable.HitOrigin);
            yield return StartCoroutine(approachingProcess);
        }
        currentReaction = EnemyReaction.None;
        MoveAround();
    }

    private IEnumerator ApproachingTo(Vector3 targetPos) {
        navMesh.speed = GetMoveSpeedFromCurrentState();
        navMesh.SetDestination(targetPos);
        while(DistanceOnXZPlane(transform.position, targetPos) > 1f) {
            yield return null;
        }
        movingState = MovingState.Stop;
        navMesh.speed = GetMoveSpeedFromCurrentState();
        navMesh.SetDestination(transform.position);
        yield return StartCoroutine(KeepTurningTo(targetPos, 5f));
    }

/*
    private IEnumerator ApproachingTo(Transform target) {
        navMesh.speed = GetMoveSpeedFromCurrentState();
        navMesh.SetDestination(target.position);
        while(DistanceOnXZPlane(transform.position, target.position) > 1f) {
            navMesh.speed = GetMoveSpeedFromCurrentState();
            navMesh.SetDestination(target.position);
            yield return null;
        }
        movingState = MovingState.Stop;
        navMesh.speed = GetMoveSpeedFromCurrentState();
        navMesh.SetDestination(transform.position);
        yield return StartCoroutine(KeepTurningTo(target.position, 5f));
    } */

    private IEnumerator ApproachingTo(Transform target) {
        StartCoroutine(KeepTurningTo(target, 5));
        while(target) {
            navMesh.speed = GetMoveSpeedFromCurrentState();
            navMesh.SetDestination(target.position);
            yield return null;
        }
        //yield return StartCoroutine(KeepTurningTo(target.position, 5f));
    }

    private IEnumerator DoDoubtProcedures(Vector3 targetPos) {
        navMesh.SetDestination(transform.position);
        yield return new WaitForSeconds(0.5f);
        approachingProcess = ApproachingTo(targetPos);
        yield return StartCoroutine(approachingProcess);
        currentReaction = EnemyReaction.None;
        MoveAround();
    }

    private IEnumerator StartApproachingTo(Vector3 targetPos, MovingState movingState) {
        this.movingState = movingState;
        StopCoroutine(approachingProcess);
        approachingProcess = ApproachingTo(targetPos);
        yield return StartCoroutine(approachingProcess);
    }

    private IEnumerator StartApproachingTo(Transform target, MovingState movingState) {
        this.movingState = movingState;
        StopCoroutine(approachingProcess);
        approachingProcess = ApproachingTo(target);
        yield return StartCoroutine(approachingProcess);
    }

    private void MoveAround() {
        StartCoroutine(KeepMovingAround());
    }

    private IEnumerator KeepMovingAround() {
        while(currentReaction == EnemyReaction.None) {
            Vector3 randomMovablePos = GetRandomMovablePos();
            yield return StartApproachingTo(randomMovablePos, MovingState.Crouching);
            yield return new WaitForSeconds(1f);
        }
    }

    private Vector3 GetRandomMovablePos() {
        Vector2 randomPos2D = UnityEngine.Random.insideUnitCircle * 5;
        Vector3 randomPos = new Vector3(transform.position.x + randomPos2D.x, 0, transform.position.z + randomPos2D.y);
        NavMeshHit hit;
        while(!NavMesh.SamplePosition(randomPos, out hit, 1, NavMesh.AllAreas)) {
            randomPos2D = UnityEngine.Random.insideUnitCircle * 5;
            randomPos = new Vector3(transform.position.x + randomPos2D.x, 0, transform.position.z + randomPos2D.y);
        }
        return hit.position;
    }

    private void TurnTo(Vector3 targetDir, float turnSpeed) {
        Vector3 direction = targetDir - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
    }

    private List<IEnemyReactable> GetEnemyReactablesInRange() {
        List<IEnemyReactable> reactables = new List<IEnemyReactable>();
        GetAllEnemyReactables(ref reactables);
        GetAll2DEnemyReactables(ref reactables);
        return reactables;
    }

    private void GetAllEnemyReactables(ref List<IEnemyReactable> reactables) {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, viewRadius);
        foreach(var collider in collidersInRange) {
            IEnemyReactable component = collider.GetComponent<IEnemyReactable>();
            if(component != null) {
                reactables.Add(component);
            }
        }
    }

    private void GetAll2DEnemyReactables(ref List<IEnemyReactable> reactables) {
        Collider2D[] collider2DsInRange = Physics2D.OverlapCircleAll(transform.position, viewRadius);
        foreach(var collider2D in collider2DsInRange) {
            IEnemyReactable component = collider2D.GetComponent<IEnemyReactable>();
            if(component != null) {
                reactables.Add(component);
            }
        }
    }

    private void FilterInvisibleReactables(ref List<IEnemyReactable> reactables) {
        List<IEnemyReactable> visibleReactables = new List<IEnemyReactable>();
        foreach(var reactable in reactables) {
            //Debug.Log(reactable + ", " + IsVisibleFromEyesight(reactable));
            if(IsVisibleFromEyesight(reactable)) {
                visibleReactables.Add(reactable);
            }
        }
        reactables = visibleReactables;
    }

    private bool IsVisibleFromEyesight(IEnemyReactable reactable) {
        if((UnityEngine.Object)reactable == this)
            return false;
        Vector3 targetPos = reactable.Position;
        Vector3 dirToTarget = (targetPos - head.position).normalized;
        //Debug.Log(reactable + ", " + Vector3.Angle(head.forward, dirToTarget));
        if (Vector3.Angle(head.forward, dirToTarget) > viewAngle / 2)
            return false;
        float dstToTarget = Vector3.Distance(head.position, targetPos);
        //Debug.Log(reactable + ", " + Physics.Raycast(head.position, dirToTarget, dstToTarget, obstacleMask));
        if (Physics.Raycast(head.position, dirToTarget, dstToTarget, obstacleMask))
            return false;
        return true;
    }

    public override void TakeHit(int damage) {
        base.TakeHit(damage);
    }

    protected override void PlayWalkingSound() {
        soundManager.PlayAudioSource3D(walkingSoundSource, transform.position);
    }

    public override void Die() {
        isAlive = false;
        GetComponent<Collider>().enabled = false;
        currentReaction = EnemyReaction.None;
        navMesh.SetDestination(transform.position);
        StopAllCoroutines();
        movingState = MovingState.Stop;
        agentAnimator.PlayDyingAnim();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == soundMask && isAlive) {
            Debug.Log(other + ", sound");
            moveTargetPos = other.transform.position;
            DetectDoubtfulPlace(moveTargetPos);
        }
    }

    private void SetFeelingColor() {
        feeling = Mathf.Clamp(feeling, 0, 100);
        Color feelingColor;
        if (feeling <= 50f) {
            feelingColor = Color.Lerp(Color.red, Color.white, feeling / 50f);
        }
        else {
            feelingColor = Color.Lerp(Color.white, Color.blue, (feeling - 50f) / 50f);
        }
        //feelingColor.a = 0.75f;
        transform.GetChild(1).GetComponent<Renderer>().material.color = feelingColor;
        //transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_RimColor", feelingColor);
    }
}
