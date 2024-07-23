using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController3D : Agent, IEnemyReactable {
    private CameraController3D cameraController;
    [SerializeField] private float defaultRotationSpeed;
    private float currentMoveSpeed;
    //private Gun gun;
    //private bool isAiming = false;
    public EnemyReaction EnemyReaction => EnemyReaction.Alert;
    public Vector3 Position => transform.position;
    public Vector3 HitOrigin => Position;
    public Transform Transform => transform;
    private Gun gun;

    protected override void Awake() {
        base.Awake();
        cameraController = Camera.main.GetComponent<CameraController3D>();
        gun = GetComponentInChildren<Gun>();//
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        CheckMovement();
        CheckMoveState();
        CheckAiming();
        CheckFiring();
        //LookingForwarWhileAiming();
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
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 moveHorizontal = cameraController.transform.right * h;
            Vector3 moveVertical = cameraController.transform.forward * v;
            moveHorizontal.y = 0;
            moveVertical.y = 0;
            Vector3 direction = (moveHorizontal + moveVertical).normalized;
            transform.Translate(direction * GetMoveSpeedFromCurrentState() * Time.deltaTime, Space.World);

            if(isAiming == false) {
                RotateTo(direction, defaultRotationSpeed);
            }
        }
    }

    private void RotateTo(Vector3 direction, float rotationSpeed) {
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        float angle = Quaternion.Angle(transform.rotation, toRotation);
        float realRotationSpeed = rotationSpeed * angle * GetMoveSpeedFromCurrentState();
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, realRotationSpeed * Time.deltaTime);
    }

    private void CheckAiming() {
        if (Input.GetMouseButton(1)) {
            isAiming = true;
            cameraController.ZoomIn();
            LookForward();
            gun.Sight();
        }
        else {
            isAiming = false;
            cameraController.ZoomOut();
            gun.CancelSight();
        }
    }

    private void CheckFiring() {
        if (Input.GetMouseButton(0)) {
            Vector3 screenCenterDir = cameraController.GetScreenCenterRay().direction;
            StartCoroutine(KeepTurningTo(transform.position + screenCenterDir, defaultRotationSpeed * 20));
            gun.PullTrigger();
        }
    }

    public override void TakeHit(int damage) {
        base.TakeHit(damage);
    }

    private void LookForward() {
        Vector3 direction = cameraController.transform.forward;
        direction.y = 0;
        RotateTo(direction, defaultRotationSpeed * 5);
    }

    protected override void PlayWalkingSound() {
        soundManager.PlayAudioSource3DWithSoundWave(walkingSoundSource, transform.position);
    }

    public override void Die() {
        throw new System.NotImplementedException();
    }
}
