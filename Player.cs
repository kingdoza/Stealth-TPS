using System;
using System.Security.Cryptography;
using UnityEngine;

public class Player : Agent, IEnemyReactable {
    [SerializeField] private float rotationSensitivity;
    [SerializeField] private Transform helmet;
    private Camera mainCamera;
    public EnemyReaction EnemyReaction { get; set; } = EnemyReaction.None;
    public Vector3 Position => helmet.position;
    public Vector3 HitOrigin => transform.position;
    public Transform Transform => transform;
    private bool canMove = true;
    private PlayerGun gun;
    private Rigidbody rigidbody;
    private bool isReloading = false;
    [SerializeField] private AudioSource reloadingAudio;
    [SerializeField] private int health;
    public Transform Head => head;
    public bool IsCrouchState { 
        get { 
            return movingState == MovingState.Crouching || movingState == MovingState.CrouchStop; 
        } 
    }
    public bool IsAiming => isAiming;
    private static Player instance;
    public static Player Instance {
        get {
            if(instance == null) {
                instance = (Player)FindObjectOfType(typeof(Player));
                if(instance == null) {
                    GameObject gameObject = new GameObject(typeof(Player).Name, typeof(Player));
                    instance = gameObject.GetComponent<Player>();
                }
            }
            return instance;
        }
    }

    protected override void Awake() {
        base.Awake();
        Cursor.visible = false;
        rigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        gun = GetComponentInChildren<PlayerGun>();
        currentHealth = maxHealth = health;
    }

    protected override void Start() {
        base.Start();
        //GameManager.Instance.uiManager.UpdateHealthStat(currentHealth);
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
        CheckReload();

        //Debug.Log(isReloading);
    }

    private void FixedUpdate() {
        //CheckMovement();
    }

    private void CheckRotation() {
        if(Input.GetAxisRaw("Mouse X") != 0) {
            float yDelta = Input.GetAxisRaw("Mouse X");
            transform.Rotate(Vector3.up * yDelta * rotationSensitivity * 0.001f);
        }
    }

    private void CheckMoveState() {
        if(Input.GetKey(KeyCode.LeftShift) && isAiming == false) {
            if(isReloading)
                CancelReload();
            movingState = MovingState.Running;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift)) {
            movingState = MovingState.Walking;
        }
        else if(Input.GetKeyDown(KeyCode.LeftControl)) {
            if(movingState == MovingState.Crouching) {
                movingState = MovingState.Walking;
            }
            else if(movingState == MovingState.CrouchStop) {
                movingState = MovingState.Stop;
            }
            else if(movingState == MovingState.Stop) {
                movingState = MovingState.CrouchStop;
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
            else if(movingState == MovingState.CrouchStop)
                movingState = MovingState.Crouching;
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 _moveHorizontal = transform.right * h;
            Vector3 _moveVertical = transform.forward * v;
            Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized;
            movingDelta = new Vector3(h, 0, v);
            rigidbody.velocity = _velocity * GetMoveSpeedFromCurrentState();
            //rigidbody.MovePosition(rigidbody.position + _velocity * GetMoveSpeedFromCurrentState() * Time.deltaTime);
            //transform.Translate(_velocity * GetMoveSpeedFromCurrentState() * Time.deltaTime, Space.World);
        }
        else {
            rigidbody.velocity = Vector3.zero;
            if(IsCrouchState)
                movingState = MovingState.CrouchStop;
            else
                movingState = MovingState.Stop;
        }
    }

    private void Aim() {
        //mainCamera.GetComponent<CameraController>().ZoomIn();
        if(isAiming == false) {
            isAiming = true;
            gun.SightAfterDelay(0.2f);
        }
        if(movingState == MovingState.Running) {
            movingState = MovingState.Walking;
        }
    }

    private void LooseAim() {
        //mainCamera.GetComponent<CameraController>().ZoomOut();
        if(isAiming == true) {
            isAiming = false;
            gun.CancelSight();
        }
    }

    private void CheckAiming() {
        if (Input.GetMouseButton(1) && !isReloading) {
            Aim();
        }
        else {
            LooseAim();
        }
    }

    private void CheckReload() {
        if (Input.GetKeyDown(KeyCode.R) && gun.CanReload() && isReloading == false) {
            StartReload();
        }
    }

    private void StartReload() {
        if(movingState == MovingState.Running) {
            movingState = MovingState.Walking;
        }
        reloadingAudio.time = 0.15f;
        reloadingAudio.Play();
        isReloading = true;
        agentAnimator.PlayReloadingAnim();
    }

    private void CheckFiring() {
        if (Input.GetMouseButtonDown(0) && !isReloading) {
            if(gun.IsMagazinEmpty() && gun.CanReload()) {
                StartReload();
                return;
            }
            agentAnimator.PlayShootAnim();
            gun.PullTrigger();
            if(gun.IsMagazinEmpty() && gun.CanReload()) {
                StartReload();
            }
        }
    }

    private void CancelReload() {
        if(isReloading == false)
            return;
        reloadingAudio.Stop();
        isReloading = false;
        agentAnimator.StopReloadingAnim();
    }

    public void Reload() {
        isReloading = false;
        gun.Reload();
    }

    public override void TakeHit(int damage) {
        base.TakeHit(damage);
        DisenableMovement();
        isAiming = false;
        CancelReload();
        gun.CancelSight();
        agentAnimator.PlayHitAnim();
        GameManager.Instance.uiManager.UpdateHealthStat(currentHealth);
    }

    public void TakeHit(Vector3 originPos, int damage) {
        TakeHit(damage);
        GameManager.Instance.uiManager.ShowHitMark(originPos);
    }

    public void TakeHit(Transform origin, int damage) {
        TakeHit(damage);
        GameManager.Instance.uiManager.ShowHitMark(origin.position);
    }

    public void DisenableMovement() {
        canMove = false;
        rigidbody.velocity = rigidbody.velocity * 0.5f;
        movingState = MovingState.Stop;
    }

    private void EnableMovement() {
        canMove = true;
    }

    protected override void PlayWalkingSound() {
        soundManager.PlayAudioSource3DWithSoundWave(walkingSoundSource, transform.position + transform.up * 0.02f);
    }

    public override void Die() {
        agentAnimator.PlayDyingAnim();
    }
}
