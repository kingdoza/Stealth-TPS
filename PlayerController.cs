using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : Agent, IEnemyReactable {
    [SerializeField] private float rotationSensitivity;
    private Camera mainCamera;
    public EnemyReaction EnemyReaction => EnemyReaction.None;
    public Vector3 Position => head.position;
    public Vector3 HitOrigin => transform.position;
    public Transform Transform => transform;
    private bool canMove = true;

    protected override void Awake() {
        base.Awake();
        Cursor.visible = false;
        mainCamera = Camera.main;
        gun = GetComponentInChildren<PlayerGun>();
    }

    protected override void Start() {
        base.Start();
        GameManager.Instance.uiManager.UpdateHealthStat(currentHealth);
    }

    protected override void Update() {
        base.Update();
        if(canMove == false) {
            movingState = MovingState.Stop;
            return;
        }
        CheckMoveState();
        CheckRotation();
        CheckMovement();
        CheckAiming();
        CheckFiring();
    }

    private void CheckRotation() {
        if(Input.GetAxisRaw("Mouse X") != 0) {
            float yDelta = Input.GetAxisRaw("Mouse X");
            transform.Rotate(Vector3.up * yDelta * rotationSensitivity * 0.001f);
        }
    }

    private void CheckMoveState() {
        if(Input.GetKey(KeyCode.LeftShift)) {
            movingState = MovingState.Running;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift)) {
            movingState = MovingState.Walking;
        }
        else if(Input.GetKeyDown(KeyCode.LeftControl)) {
            if(movingState == MovingState.Crouching) {
                movingState = MovingState.Walking;
            }
            else {
                movingState = MovingState.Crouching;
            }
        }
    }

    private void CheckMovement() {
        if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
            if(movingState == MovingState.Stop)
                movingState = MovingState.Walking;
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 _moveHorizontal = transform.right * h;
            Vector3 _moveVertical = transform.forward * v;
            Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized;
            movingDelta = new Vector3(h, 0, v);
            transform.Translate(_velocity * GetMoveSpeedFromCurrentState() * Time.deltaTime, Space.World);
        }
        else {
            movingState = MovingState.Stop;
        }
    }

    private void CheckAiming() {
        if (Input.GetMouseButton(1)) {
            mainCamera.GetComponent<CameraController>().ZoomIn();
            if(isAiming == false) {
                isAiming = true;
                gun.SightAfterDelay(0.2f);
            }
        }
        else {
            mainCamera.GetComponent<CameraController>().ZoomOut();
            if(isAiming == true) {
                isAiming = false;
                gun.CancelSight();
            }
        }
    }

    private void CheckFiring() {
        if (Input.GetMouseButtonDown(0)) {
            gun.PullTrigger();
        }
    }

    public override void TakeHit(int damage) {
        base.TakeHit(damage);
        DisenableMovement();
        isAiming = false;
        gun.CancelSight();
        agentAnimator.PlayHitAnim();
        GameManager.Instance.uiManager.UpdateHealthStat(currentHealth);
    }

    public void DisenableMovement() {
        canMove = false;
        movingState = MovingState.Stop;
    }

    private void EnableMovement() {
        canMove = true;
    }

    protected override void PlayWalkingSound() {
        soundManager.PlayAudioSource3DWithSoundWave(walkingSoundSource, transform.position + transform.up * 0.02f);
    }

    public override void Die() {
        
    }
}
