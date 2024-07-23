using UnityEngine;

public class CameraController : MonoBehaviour, IStageRun {
    private Transform follower;
    private float rotateSmooth = 100f;
    private float zoomSpeed = 10f;
    private float crouchSpeed = 10f;
    [SerializeField] private Vector3 normalOffset = new Vector3(0, 2.0f, -3.42f);
    [SerializeField] private Vector3 zoomInOffset;
    [SerializeField] private float crouchOffset = 0.3f;
    private Vector3 zoomOffsetGap;
    private Vector3 currentOffset;
    private Vector3 targetOffset;
    [SerializeField] private float normalAngleX = 24.7f;
    [SerializeField] private float zoomInAngleX;
    private float currentAngleX;
    private float targetAngleX;
    [SerializeField] private LayerMask collisionMask;
    public bool IsRunning { get; set; } = false;

    private void Awake() {
        //normalAngleX = transform.rotation.eulerAngles.x;
        //zoomInAngleX = normalAngleX - 10f;
        currentAngleX = normalAngleX;

        //normalOffset = transform.position - player.position;
        //zoomInOffset = normalOffset + new Vector3(0, -1f, 1f);
        currentOffset = normalOffset;
        zoomOffsetGap = zoomInOffset - normalOffset;
    }

    // private void Start() {
    //     follower = Player.Instance.Head;
    //     //player = GameManager.Instance.stageManager.Player;
    // }

    public void Run() {
        IsRunning = true;
        follower = Player.Instance.Head;
    }

    private void LateUpdate() {
        if(IsRunning == false)
            return;
        Vector3 targetRotation = new Vector3(currentAngleX, follower.rotation.eulerAngles.y);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), rotateSmooth * Time.deltaTime);
        Vector3 targetPosition = follower.position + follower.rotation * currentOffset;
        //transform.position = Vector3.Lerp(transform.position, targetPosition, rotateSmooth * Time.deltaTime);

        Vector3 desiredPosition = Vector3.Lerp(transform.position, targetPosition, rotateSmooth * Time.deltaTime);

        Vector3 collisionDetectionDir = desiredPosition - follower.position;
        Ray ray = new Ray(follower.position, collisionDetectionDir);
        Debug.DrawRay(ray.origin, ray.direction * collisionDetectionDir.magnitude, Color.red, 1);
        if(Physics.Raycast(ray, out RaycastHit hit, collisionDetectionDir.magnitude, collisionMask)) {
            desiredPosition = hit.point;
        }
        transform.position = desiredPosition;

        SetCameraSubMovement();
    }

    private void SetCameraSubMovement() {
        if(Player.Instance.IsCrouchState && Player.Instance.IsAiming) {
            targetAngleX = zoomInAngleX;
            targetOffset = zoomInOffset - transform.up * crouchOffset;
        }
        else if(Player.Instance.IsCrouchState) {
            targetAngleX = normalAngleX;
            targetOffset = normalOffset - transform.up * crouchOffset;
        }
        else if(Player.Instance.IsAiming) {
            targetAngleX = zoomInAngleX;
            targetOffset = zoomInOffset;
        }
        else {
            targetAngleX = normalAngleX;
            targetOffset = normalOffset;
        }
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, zoomSpeed * Time.deltaTime);
        currentAngleX = Mathf.Lerp(currentAngleX, targetAngleX, zoomSpeed * Time.deltaTime);
    }

    public void ZoomIn() {
        currentAngleX = Mathf.Lerp(currentAngleX, zoomInAngleX, zoomSpeed * Time.deltaTime);
        currentOffset = Vector3.Lerp(currentOffset, zoomInOffset, zoomSpeed * Time.deltaTime);
        //currentOffset = Vector3.Lerp(currentOffset, offset + zoomOffsetGap, zoomSpeed * Time.deltaTime);
    }

    public void ZoomOut() {
        currentAngleX = Mathf.Lerp(currentAngleX, normalAngleX, zoomSpeed * Time.deltaTime);
        currentOffset = Vector3.Lerp(currentOffset, normalOffset, zoomSpeed * Time.deltaTime);
        //currentOffset = Vector3.Lerp(currentOffset, offset - zoomOffsetGap, zoomSpeed * Time.deltaTime);
    }
}
