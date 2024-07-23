using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class EnemyVariables {
    [SerializeField] private int feeling;
    public int Feeling => feeling;
    [SerializeField] private int importance;
    public int Importance => importance;

    public EnemyVariables(int feeling, int importance) {
        this.feeling = feeling;
        this.importance = importance;
    }
}

public class Enemy : Agent, IEnemyReactable {
    [SerializeField] private float viewRadius;
    [SerializeField] [Range(0, 360)] private float viewAngle;
    [SerializeField] private LayerMask obstacleMask;
    public Vector3 Position => transform.position;
    public bool IsAlive { get; protected set; }
    protected EnemyReaction currentReaction;
    public EnemyReaction Reaction { get { return currentReaction; } 
        set {
            if(IsAlive == false)
                return;
            currentReaction = value;
            ResetReactionSequence();
        }
    }
    private NavMeshAgent navMesh;
    private Vector3 moveTargetPos;
    protected IEnumerator approachingProcess;
    private Vector3 previousPos;
    public Transform Head => head;
    [SerializeField] private LayerMask soundMask;
    [SerializeField] private int importance = 50;
    public int Importance { get; set; }
    [SerializeField] private int feeling = 50; //0~49 50 51~100
    public int Feeling { get => feeling; 
        set {
            feeling = value;
            SetFeelingColor();
        }
    }
    private Punch punch;
    private bool isAttacked = false;
    private NavMeshPath path;
    private BT_Node behaviorRoot;
    //protected PlayerController player;
    [SerializeField] private float playerPosUpdateDelay = 0.2f;
    private float playerPosUpdateRemained = 0;
    private Vector3 destination;
    private IEnemyReactable reactable = null;
    protected MovingState MovingState { 
        set { 
            if(value != movingState) {
                movingState = value;
                navMesh.speed = GetMoveSpeedFromCurrentState();
            }
        }
    }
    private EnemyReaction enemyReaction = EnemyReaction.None;
    public EnemyReaction EnemyReaction => enemyReaction;
    public Transform Transform => transform;
    private Vector3 hitOrigin;
    public Vector3 HitOrigin { 
        get {
            enemyReaction = EnemyReaction.None;
            return hitOrigin;
        }
    }
    [SerializeField] private float rotationSpeed = 3;
    protected Sequence alertSequence;
    private Sequence searchSequence;
    private Sequence doubtSequence;
    private Sequence patrolSequence;
    private SoundHeard soundHeard;
    private bool isAutoRotate = true;

    protected override void Awake() {
        punch = GetComponent<Punch>();
        base.Awake();
        SetFeelingColor();
        path = new NavMeshPath();
        soundHeard = transform.parent.GetComponentInChildren<SoundHeard>();
        //gun = GetComponentInChildren<EnemyGun>();
    }

    protected override void Start() {
        //SetWeaponToPunch();
        base.Start();
        IsAlive = true;
        previousPos = transform.position;
        approachingProcess = ApproachingTo(transform.position);
        navMesh = GetComponent<NavMeshAgent>();
        currentReaction = EnemyReaction.None;
        //StartCoroutine(KeepWatchingForward());
        //player = GameManager.Instance.stageManager.Player;
        SetAlertSequence();
        SetBehaviorTree();

        navMesh.updateRotation = false;

        //MoveAround();
    }

    protected override void Update() {
        base.Update();
        //CalculateMovingDir();
        //OnDrawGizmosSelected();
        //Debug.Log(movingState);
        //Debug.Log(navMesh.speed);
        //Debug.Log(currentReaction);
        if((this is ArmedEnemy) == false)
            punch.UpdatePunchAnimState(agentAnimator.IsPunchingAnimPlaying());

        if(Input.GetKeyDown(KeyCode.Space)) {
            Player.Instance.EnemyReaction = EnemyReaction.Alert;
            TryChangeReaction(Player.Instance);
            //currentReaction = EnemyReaction.Alert;
        }

        //WatchForward();
        //behaviorRoot.Execute();
    }

    private void FixedUpdate() {
        if(IsAlive == false)
            return;
        behaviorRoot.Execute();
        if(isAutoRotate == false)
            return;
        Vector3 lookrotation = navMesh.steeringTarget - transform.position;
        if(lookrotation != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(lookrotation), rotationSpeed * Time.fixedDeltaTime); 
    }

/*
    private void SetBehaviorTree() {
        behaviorRoot = new ServiceNode(WatchForward, Time.fixedDeltaTime,
            new Selector(new List<BT_Node> {
                new Sequence(new List<BT_Node> {
                    new ConditionNode(() => currentReaction == EnemyReaction.Alert),
                    new OnceNode(new ActionNode(SetMoveStateToStop)),
                    new OnceNode(new ActionNode(SetPlayerPos)),
                    new OnceNode(new ActionNode(TurnRapidly)),
                    new OnceNode(new ActionNode(SetMoveStateToRunning)),
                    new ActionNode(ChasePlayer)
                }),
                new Sequence(new List<BT_Node> {
                    new ConditionNode(() => currentReaction == EnemyReaction.Search),
                    new OnceNode(new ActionNode(SetMoveStateToStop)),
                    new OnceNode(new ActionNode(SetReatableHitOriginPos)),
                    new OnceNode(new ActionNode(TurnRapidly)),
                    new OnceNode(new WaitNode(0.5f)),
                    new OnceNode(new ActionNode(SetMoveStateToWalking)),
                    new ActionNode(Approach)
                }),
                new Sequence(new List<BT_Node> {
                    new ConditionNode(() => currentReaction == EnemyReaction.Doubt),
                    new OnceNode(new ActionNode(SetMoveStateToStop)),
                    new OnceNode(new ActionNode(SetReatablePos)),
                    new OnceNode(new ActionNode(TurnRapidly)),
                    new OnceNode(new WaitNode(0.5f)),
                    new OnceNode(new ActionNode(SetMoveStateToCrouching)),
                    new OnceNode(new ActionNode(Approach)),
                    new Sequence(new List<BT_Node> {
                        new OnceNode(new ActionNode(SetMoveStateToStop)),
                        new OnceNode(new WaitNode(0.5f)),
                        new OnceNode(new ActionNode(SetReatableHitOriginPos)),
                        new OnceNode(new ActionNode(TurnRapidly)),
                        new OnceNode(new ActionNode(SetMoveStateToCrouching)),
                        new OnceNode(new ActionNode(Approach))
                    })
                }),
                new Sequence(new List<BT_Node> {
                    new ActionNode(SetPatrolPos),
                    new ActionNode(SetMoveStateToCrouching),
                    new ActionNode(Approach)
                })
            })
        );
    } */

    protected virtual void SetAlertSequence() {
        alertSequence = new Sequence(new List<BT_Node> {
            //new ConditionNode(() => currentReaction == EnemyReaction.Alert),
            new OnceNode(new ActionNode(SetMoveStateToStop)),
            new OnceNode(new ActionNode(SetPlayerPos)),
            new OnceNode(new ActionNode(TurnRapidly)),
            new ActionNode(SetChaseSpeed),
            new ActionNode(ChasePlayer),
            new ActionNode(PunchPlayer)
        });
    }


    private void SetBehaviorTree() {
        searchSequence = new Sequence(new List<BT_Node> {
            //new ConditionNode(() => currentReaction == EnemyReaction.Search),
            new OnceNode(new ActionNode(SetMoveStateToStop)),
            new OnceNode(new ActionNode(SetReatableHitOriginPos)),
            new OnceNode(new ActionNode(TurnRapidly)),
            new OnceNode(new WaitNode(0.5f)),
            new OnceNode(new ActionNode(SetMoveStateToWalking)),
            new ActionNode(Approach),
            new ActionNode(SetReactionStateToNone)
        });

        doubtSequence = new Sequence(new List<BT_Node> {
            //new ConditionNode(() => currentReaction == EnemyReaction.Doubt),
            new OnceNode(new ActionNode(SetMoveStateToStop)),
            new OnceNode(new ActionNode(SetReatablePos)),
            new OnceNode(new ActionNode(TurnRapidly)),
            new OnceNode(new WaitNode(0.5f)),
            new OnceNode(new ActionNode(SetMoveStateToCrouching)),
            new OnceNode(new ActionNode(Approach)),
            new Sequence(new List<BT_Node> {
                new OnceNode(new ActionNode(SetMoveStateToStop)),
                new OnceNode(new WaitNode(0.5f)),
                new OnceNode(new ActionNode(SetReatableHitOriginPos)),
                new OnceNode(new ActionNode(TurnRapidly)),
                new OnceNode(new ActionNode(SetMoveStateToCrouching)),
                new OnceNode(new ActionNode(Approach))
            }),
            new ActionNode(SetReactionStateToNone)
        });

        patrolSequence = new Sequence(new List<BT_Node> {
            new OnceNode(new ActionNode(SetMoveStateToStop)),
            new OnceNode(new WaitNode(0.3f)),
            new ActionNode(SetPatrolPos),
            new ActionNode(SetMoveStateToCrouching),
            new ActionNode(Approach)
        });

        ActionNode stateToNoneAction = new ActionNode(SetReactionStateToNone);
        behaviorRoot = new ServiceNode(WatchForward, Time.fixedDeltaTime,
            new Selector(new List<BT_Node> {
                new Sequence(new List<BT_Node> {
                    new ConditionNode(() => currentReaction == EnemyReaction.Alert),
                    alertSequence
                }),
                new Sequence(new List<BT_Node> {
                    new ConditionNode(() => currentReaction == EnemyReaction.Search),
                    new ConditionalRunDecorator(searchSequence, stateToNoneAction),
                }),
                new Sequence(new List<BT_Node> {
                    new ConditionNode(() => currentReaction == EnemyReaction.Doubt),
                    new ConditionalRunDecorator(doubtSequence, stateToNoneAction),
                }),
                new Sequence(new List<BT_Node> {
                    new ConditionNode(() => currentReaction == EnemyReaction.None),
                    patrolSequence,
                })
                //patrolSequence

                //new AlwaysRunDecorator(alertSequence, SetReactionStateToNone),
                //new AlwaysRunDecorator(searchSequence, SetReactionStateToNone),
                //new AlwaysRunDecorator(doubtSequence, SetReactionStateToNone),
                //new AlwaysRunDecorator(patrolSequence, SetReactionStateToNone),
                // alertSequence,
                // searchSequence,
                // doubtSequence,
                // patrolSequence
            })
        );
    }

    private BT_NodeState PunchPlayer() {
        Debug.Log("Punch");
        if(movingState != MovingState.Stop)
            return BT_NodeState.Failure;
        if(punch.TryStartHit()) {
            agentAnimator.PlayPunchingAnim();
            return BT_NodeState.Success;
        }
        return BT_NodeState.Failure;
    }

    protected virtual BT_NodeState SetChaseSpeed() {
        if(Vector3.Distance(Player.Instance.Position, transform.position) <= 1.6f || punch.IsPunching)
            return SetMoveStateToStop();
        //punch.ResetHitDelay();
        return SetMoveStateToRunning();
    }

    protected BT_NodeState ChasePlayer() {
        Debug.Log("ChasePlayer");
        if(playerPosUpdateRemained <= 0) {
            playerPosUpdateRemained = playerPosUpdateDelay;
            navMesh.SetDestination(Player.Instance.Position);
        }
        playerPosUpdateRemained -= Time.deltaTime;
        LookAt(Player.Instance.Position);
        Debug.Log(Vector3.Distance(Player.Instance.Position, transform.position));
        return BT_NodeState.Success;
        if(Vector3.Distance(Player.Instance.Position, transform.position) < 1f) {
            return BT_NodeState.Success;
        }
        return BT_NodeState.Running;
    }

    protected BT_NodeState TurnRapidly() {
        Debug.Log("TurnRapidly");
        isAutoRotate = false;
        Vector3 direction = ProjectToXZPlane(navMesh.destination - transform.position);
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 8);;
        Debug.DrawLine(transform.position, navMesh.destination, Color.red);
        Debug.Log(Quaternion.Angle(transform.rotation, rotation));
        if(Quaternion.Angle(transform.rotation, rotation) > 1f)
            return BT_NodeState.Running;
        transform.rotation = rotation;
        isAutoRotate = true;
        return BT_NodeState.Success;
    }

    private BT_NodeState Approach() {
        Debug.Log("Approach");
        if(navMesh.destination != destination)
            navMesh.SetDestination(destination);
        //Debug.DrawLine(transform.position, destination, Color.yellow);
        //Debug.Log(DistanceOnXZPlane(transform.position, navMesh.destination));
        if(DistanceOnXZPlane(transform.position, navMesh.destination) > 1f)
            return BT_NodeState.Running;
        MovingState = MovingState.Stop;
        return BT_NodeState.Success;
    }

    private BT_NodeState SetPatrolPos() {
        if(DistanceOnXZPlane(transform.position, navMesh.destination) > 1f)
            return BT_NodeState.Running;

        Debug.Log("SetPatrolPos");
        Vector3 randomMovablePos = GetRandomMovablePos();
        destination = randomMovablePos;
        return BT_NodeState.Success;
    }

    private BT_NodeState LookDestination() {
        LookAt(destination);
        return BT_NodeState.Success;
    }

    private BT_NodeState SetReatablePos() {
        if(reactable.Position == null)
            return BT_NodeState.Failure;
        Debug.Log(reactable +  " : SetReatablePos");
        destination = reactable.Position;
        navMesh.SetDestination(destination);
        return BT_NodeState.Success;
    }

    protected BT_NodeState SetPlayerPos() {
        Debug.Log("SetPlayerPos");
        destination = Player.Instance.Position;
        navMesh.SetDestination(destination);
        return BT_NodeState.Success;
    }

    private BT_NodeState Stop() {
        movingState = MovingState.Stop;
        navMesh.SetDestination(transform.position);
        return BT_NodeState.Success;
    }

    private BT_NodeState SetReatableHitOriginPos2() {
        if(reactable.HitOrigin == reactable.Position)
            return BT_NodeState.Failure;
        Debug.Log("SetReatableHitOriginPos");
        destination = reactable.HitOrigin;
        navMesh.SetDestination(destination);
        return BT_NodeState.Success;
    }

    private BT_NodeState SetReatableHitOriginPos() {
        if(reactable == null)
            return BT_NodeState.Failure;
        if(reactable.HitOrigin == reactable.Position)
            return BT_NodeState.Failure;
        Debug.Log("SetReatableHitOriginPos");
        destination = reactable.HitOrigin;
        navMesh.SetDestination(destination);
        reactable = null;
        return BT_NodeState.Success;
    }

    private BT_NodeState SetReactionStateToNone() {
        Reaction = EnemyReaction.None;
        return BT_NodeState.Success;
    }

    private BT_NodeState SetMoveStateToRunning() {
        MovingState = MovingState.Running;
        return BT_NodeState.Success;
    }

    private BT_NodeState SetMoveStateToWalking() {
        MovingState = MovingState.Walking;
        return BT_NodeState.Success;
    }

    private BT_NodeState SetMoveStateToCrouching() {
        MovingState = MovingState.Crouching;
        return BT_NodeState.Success;
    }

    protected BT_NodeState SetMoveStateToStop() {
        MovingState = MovingState.Stop;
        return BT_NodeState.Success;
    }

    protected void OnDrawGizmosSelected() {
    if (navMesh == null)
        return;
    Gizmos.color = Color.red;
    Gizmos.DrawSphere(navMesh.destination, 0.5f);
    Gizmos.color = Color.green;
    Gizmos.DrawLine(navMesh.transform.position, navMesh.destination);
}

    private void CalculateMovingDir() {
        Vector3 currentPos = transform.position;
        Vector3 posGap = currentPos - previousPos;
        Vector3 localPosGap = transform.InverseTransformDirection(posGap);
        movingDelta = localPosGap;
        previousPos = currentPos;
    }

    private void WatchForward() {
        if(IsAlive == false)
            return;
        List<IEnemyReactable> reactables = GetEnemyReactablesInRange();
        //reactables.RemoveAll(element => element.EnemyReaction == EnemyReaction.None);
        //reactables = reactables.Where(IsVisibleFromEyesight).ToList();
        IEnemyReactable sound = HearSound(reactables);
        FilterInvisibleReactables(ref reactables);
        if(isAttacked) {
            reactables.Add(Player.Instance);
            isAttacked = false;
        }
        if(sound != null)
            reactables.Add(sound);
        reactables = reactables.OrderBy(obj => obj.EnemyReaction).ToList();
        //foreach(var reactable in reactables) {
            //Debug.Log(reactable);
        //}
        if(reactables.Count <= 0)
            return;
        IEnemyReactable reactableToReact = reactables[0];
        if(reactableToReact is SoundWave soundWave) {
            soundHeard.transform.position = soundWave.Position;
            reactableToReact = soundHeard;
        }
        TryChangeReaction(reactableToReact);
    }

    private IEnemyReactable HearSound(List<IEnemyReactable> reactables) {
        for(int i = 0; i < reactables.Count; ++i) {
            if((reactables[i] is SoundWave) == false)
                continue;
            SoundWave soundWave = (SoundWave)reactables[i];
            if(Vector3.Distance(Position, soundWave.Position) <= soundWave.GetSpreadDistance()) {
                return soundWave;
            }
        }
        return null;
    }

    private void TryChangeReaction(IEnemyReactable reactable) {
        if(reactable.EnemyReaction < currentReaction) {
            this.reactable = reactable;
            Reaction = reactable.EnemyReaction;
            //StopCoroutine(approachingProcess);
            //ExecuteReaction(reactable);
            //Debug.Log(currentReaction);
        }
    }

    private void ResetReactionSequence() {
        Action action = currentReaction switch {
            EnemyReaction.Alert => alertSequence.ResetOnceNodes,
            EnemyReaction.Search => searchSequence.ResetOnceNodes,
            EnemyReaction.Doubt => doubtSequence.ResetOnceNodes,
            EnemyReaction.None => patrolSequence.ResetOnceNodes,
            _ => null
        };
        if(action != null)
            action();
    }

    private Vector3 GetRandomMovablePos() {
        Vector2 randomPos2D = UnityEngine.Random.insideUnitCircle * 5;
        Vector3 randomPos = new Vector3(transform.position.x + randomPos2D.x, 0, transform.position.z + randomPos2D.y);
        NavMeshHit hit;
        while(!NavMesh.SamplePosition(randomPos, out hit, 1, NavMesh.AllAreas) || !IsPathToDestination(randomPos)) {
            randomPos2D = UnityEngine.Random.insideUnitCircle * 5;
            randomPos = new Vector3(transform.position.x + randomPos2D.x, 0, transform.position.z + randomPos2D.y);
        }
        return hit.position;
    }

    private bool IsPathToDestination(Vector3 targetPos) {
        if (NavMesh.CalculatePath(transform.position, targetPos, NavMesh.AllAreas, path) == false)
            return false;
        if(path.status == NavMeshPathStatus.PathInvalid)
            return false;
        return true;
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

    protected bool IsVisibleFromEyesight(IEnemyReactable reactable) {
        if((UnityEngine.Object)reactable == this)
            return false;
        if(reactable is SoundWave)
            return false;
        Vector3 targetPos = reactable.Position;
        Vector3 dirToTarget = (targetPos - head.position).normalized;
        //Debug.Log(reactable + ", " + Vector3.Angle(head.forward, dirToTarget));
        if (Vector3.Angle(head.forward, dirToTarget) > viewAngle / 2)
            return false;
        float dstToTarget = Vector3.Distance(head.position, targetPos);
        Debug.DrawRay(head.position, dirToTarget * viewRadius, Color.yellow);
        //Debug.Log(reactable + ", " + Physics.Raycast(head.position, dirToTarget, dstToTarget, obstacleMask));
        if (Physics.Raycast(head.position, dirToTarget, dstToTarget, obstacleMask))
            return false;
        return true;
    }

    public override void TakeHit(int damage) {
        currentReaction = EnemyReaction.Alert;

        
        enemyReaction = EnemyReaction.Search;
        hitOrigin = Player.Instance.Position;
        CancelInvoke(nameof(SetOriginReactable));
        Invoke(nameof(SetOriginReactable), 0.2f);
        base.TakeHit(damage);
        isAttacked = true;
        if(IsAlive == false)
            return;
        //enemyReaction = EnemyReaction.Search;
        //hitOrigin = player.Position;
        //CancelInvoke(nameof(SetToNonReactable));
        //Invoke(nameof(SetToNonReactable), 0.2f);
    }

    private IEnumerator SetToNonReactableAfterDelay() {
        if(IsAlive == false)
            yield break;
        yield return new WaitForSeconds(0.2f);
        Debug.Log("NonReact");
        enemyReaction = EnemyReaction.None;
    }

    private void SetOriginReactable() {
        if(IsAlive)
            enemyReaction = EnemyReaction.None;
        else
            enemyReaction = EnemyReaction.Doubt;
    }

    protected override void PlayWalkingSound() {
        soundManager.PlayAudioSource3D(walkingSoundSource, transform.position);
    }

    public override void Die() {
        IsAlive = false;
        GetComponent<Collider>().isTrigger = true;
        GetComponent<BoxCollider>().center = Vector3.up * -0.5f;
        currentReaction = EnemyReaction.None;
        StopAllCoroutines();
        MovingState = MovingState.Stop;
        agentAnimator.PlayDyingAnim();
        hitOrigin = Player.Instance.Position;
        //enemyReaction = EnemyReaction.Doubt;
    }

/*
    private void OnTriggerEnter(Collider other) {
        if(((1 << other.gameObject.layer) & soundMask) != 0 && IsAlive) {
            Debug.Log(other.ToString());
            //DetectDoubtfulPlace(moveTargetPos);
        }
    } */

    protected virtual void SetFeelingColor() {
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

    private IEnumerator KeepWatchingForward() {
        while(IsAlive) {
            List<IEnemyReactable> reactables = GetEnemyReactablesInRange();
            //reactables.RemoveAll(element => element.EnemyReaction == EnemyReaction.None);
            //reactables = reactables.Where(IsVisibleFromEyesight).ToList();
            FilterInvisibleReactables(ref reactables);
            if(isAttacked) {
                reactables.Add(Player.Instance);
                isAttacked = false;
            }
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

    protected virtual void Alert(IEnemyReactable reactable) {
        //Debug.Log(reactable);
        StopCoroutine(approachingProcess);
        StartCoroutine(DoAlertProcedures(reactable));
    }

    private IEnumerator DoAlertProcedures(IEnemyReactable reactable) {
        Debug.Log("Alert Prodecure Start");
        yield return StartCoroutine(KeepTurningTo(reactable.Position, 10f));
        StartCoroutine(StartApproachingTo(reactable.Transform, MovingState.Crouching));
        while(currentReaction == EnemyReaction.Alert) {
            yield return null;
            AdjustAlertMoveSpeedTo(reactable);
            //TryPunch(reactable);
        }
        Debug.Log("Alert Prodecure End");
    }

    private void TryPunch(IEnemyReactable reactable) {
        float distance = Vector3.Distance(reactable.Position, transform.position);
        //agentAnimator.PlayPunchingPrepareAnim();
        if(punch.TryStartHit()) {
            agentAnimator.PlayPunchingAnim();
        }
    }

    protected virtual void AdjustAlertMoveSpeedTo(IEnemyReactable reactable) {
        float distanceToTarget = Vector3.Distance(transform.position, reactable.Transform.position);
        if(distanceToTarget >= 1.2f) {
            movingState = MovingState.Running;
            punch.ResetHitDelay();
        }
        else {
            movingState = MovingState.Stop;
            TryPunch(reactable);
        }
    }

    private void Search(IEnemyReactable reactable) {
        //Debug.Log(reactable);
        StartCoroutine(DoSearchProcedures(reactable));
        //근원지로 바로 바라봄
        //근원지로 중간 속도로 이동
    }
/*
    private IEnumerator DoSearchProcedures(IEnemyReactable reactable) {
        navMesh.SetDestination(transform.position);
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(KeepTurningTo(reactable.HitOrigin, 10f));
        yield return new WaitForSeconds(1f);
        approachingProcess = ApproachingTo(reactable.HitOrigin);
        yield return StartCoroutine(approachingProcess);
        currentReaction = EnemyReaction.None;
        MoveAround();
    } */

    private IEnumerator DoSearchProcedures(IEnemyReactable reactable) {
        Debug.Log("Search Prodecure Start");
        movingState = MovingState.Stop;
        navMesh.SetDestination(transform.position);
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(KeepTurningTo(reactable.HitOrigin, 10f));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(StartApproachingTo(reactable.HitOrigin, MovingState.Walking));
        currentReaction = EnemyReaction.None;
        MoveAround();
        Debug.Log("Search Prodecure End");
    }

    private void Doubt(IEnemyReactable reactable) {
        //Debug.Log(reactable);
        movingState = MovingState.Crouching;
        StartCoroutine(DoDoubtProcedures(reactable));
        //물체로 천천히 이동
        //근원지로 느린 속도로 이동
    }

    private void DetectDoubtfulPlace(Vector3 targetPos) {
        if(currentReaction < EnemyReaction.Doubt)
            return;
        //Debug.Log(targetPos + ", sound");
        currentReaction = EnemyReaction.Doubt;
        StartCoroutine(DoDoubtProcedures(targetPos));
    }
/*
    private void DetectDoubtfulPlace(Vector3 targetPos) {
        if(currentReaction < EnemyReaction.Doubt)
            return;
        Debug.Log(targetPos + ", sound");
        movingState = MovingState.Crouching;
        currentReaction = EnemyReaction.Doubt;
        StopCoroutine(approachingProcess);
        StartCoroutine(DoDoubtProcedures(targetPos));
    } */

/*
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
    } */

    private IEnumerator DoDoubtProcedures(IEnemyReactable reactable) {
        Debug.Log("Doubt Prodecure Start");
        movingState = MovingState.Stop;
        navMesh.SetDestination(transform.position);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(StartApproachingTo(reactable.Position, MovingState.Crouching));
        yield return new WaitForSeconds(1f);
        if(reactable.HitOrigin != reactable.Position) {
            yield return StartCoroutine(StartApproachingTo(reactable.HitOrigin, MovingState.Crouching));
        }
        currentReaction = EnemyReaction.None;
        MoveAround();
        Debug.Log("Doubt Prodecure End");
    }

    private IEnumerator ApproachingTo(Vector3 targetPos) {
        Debug.Log("Approach Start");
        navMesh.speed = GetMoveSpeedFromCurrentState();
        navMesh.SetDestination(targetPos);
        while(DistanceOnXZPlane(transform.position, navMesh.destination) > 1f) {
            Debug.DrawLine(transform.position, navMesh.destination, Color.red);
            yield return null;
        }
        movingState = MovingState.Stop;
        navMesh.speed = GetMoveSpeedFromCurrentState();
        navMesh.SetDestination(transform.position);
        yield return StartCoroutine(KeepTurningTo(targetPos, 5f));
        Debug.Log("Approach End");
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
/*
    private IEnumerator DoDoubtProcedures(Vector3 targetPos) {
        navMesh.SetDestination(transform.position);
        yield return new WaitForSeconds(0.5f);
        approachingProcess = ApproachingTo(targetPos);
        yield return StartCoroutine(approachingProcess);
        currentReaction = EnemyReaction.None;
        MoveAround();
    } */

    private IEnumerator DoDoubtProcedures(Vector3 targetPos) {
        movingState = MovingState.Stop;
        navMesh.SetDestination(transform.position);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(StartApproachingTo(targetPos, MovingState.Crouching));
        currentReaction = EnemyReaction.None;
        MoveAround();
    }

    protected IEnumerator StartApproachingTo(Vector3 targetPos, MovingState movingState) {
        this.movingState = movingState;
        StopCoroutine(approachingProcess);
        approachingProcess = ApproachingTo(targetPos);
        yield return StartCoroutine(approachingProcess);
    }

    protected IEnumerator StartApproachingTo(Transform target, MovingState movingState) {
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

    private void TurnTo(Vector3 targetDir, float turnSpeed) {
        Vector3 direction = targetDir - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
    }
}
