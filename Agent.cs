using System.Collections;
using UnityEngine;

public enum MovingState {
    Running = 1, Walking, Crouching, Stop
}

public abstract class Agent : MonoBehaviour {
    protected int maxHealth = 10;
    protected int currentHealth;
    protected AudioSource walkingSoundSource;
    protected SoundManager soundManager;
    [SerializeField] private float maxWalkingDistance = 10;
    [SerializeField] private float crouchSpeed = 2;
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float runSpeed = 10;
    protected float currentWalkingDistance = 0;
    protected AgentAnimator agentAnimator;
    protected MovingState movingState = MovingState.Stop;
    protected Vector3 movingDelta;
    protected bool isAiming = false;
    protected Gun gun;

    protected virtual void Awake() {
        gun = GetComponentInChildren<Gun>();
        agentAnimator = GetComponent<AgentAnimator>();
        walkingSoundSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
    }

    protected virtual void Start() {
        soundManager = GameManager.Instance.soundManager;
        StartCoroutine(KeepMakingFootstepSound());
    }

    protected virtual void Update() {
        agentAnimator.SetMovingAnim(movingState, movingDelta);
        agentAnimator.SetTurningAnim(transform.rotation.eulerAngles);
        agentAnimator.SetAimAnim(isAiming);
    }

    protected float GetMoveSpeedFromCurrentState() {
        float moveSpeed = movingState switch {
            MovingState.Running => runSpeed,
            MovingState.Walking => walkSpeed,
            MovingState.Crouching => crouchSpeed,
            _ => 0
        };
        return moveSpeed;
    }

    private IEnumerator KeepMakingFootstepSound() {
        while(gameObject) {
            Vector3 previousPos = transform.position;
            yield return new WaitForSeconds(0.5f);
            Vector3 currentPos = transform.position;
            float distanceWalked = Vector3.Distance(currentPos, previousPos);
            if(distanceWalked < 0.1f)
                continue;
            walkingSoundSource.volume = GetWalkingVolume(distanceWalked);
            PlayWalkingSound();
        }
    }

    protected IEnumerator KeepTurningTo(Vector3 targetPos, float turnSpeed) {
        Vector3 direction = ProjectToXZPlane(targetPos - transform.position);
        Quaternion rotation = Quaternion.LookRotation(direction);
        while(Quaternion.Angle(transform.rotation, rotation) > 0.1f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
            yield return null;
        }
        transform.rotation = rotation;
    }

    protected void LookAt(Vector3 targetPos) {
        targetPos = ProjectToXZPlane(targetPos);
        transform.LookAt(targetPos);
    }

    protected float DistanceOnXZPlane(Vector3 pos1, Vector3 pos2) {
        Vector3 ProjectionOfPos1 = ProjectToXZPlane(pos1);
        Vector3 ProjectionOfPos2 = ProjectToXZPlane(pos2);
        return Vector3.Distance(ProjectionOfPos1, ProjectionOfPos2);
    }

    protected Vector3 ProjectToXZPlane(Vector3 origin) {
        return new Vector3(origin.x, 0, origin.z);
    }

    protected abstract void PlayWalkingSound();

    private float GetWalkingVolume(float distanceWalked) {
        float walkingVolume = distanceWalked / maxWalkingDistance;
        if(walkingVolume > 1)
            walkingVolume = 1;
        return walkingVolume;
    }

    protected virtual void TakeHit(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            Die();
        }
    }

    public abstract void Die();
}
