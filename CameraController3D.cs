using UnityEngine;
using System.Collections;

public class CameraController3D : MonoBehaviour, IStageRun {
    public float xAxis;
    public float yAxis;
    [SerializeField] private Transform target;
    [SerializeField] private float rotSensitive = 3f;
    [SerializeField] private float normalRotationX = 24f;
    [SerializeField] private float zoomRotationX = 0f;
    [SerializeField] private float normalDistance = 2f;
    [SerializeField] private float zoomDistance = 1.2f;
    [SerializeField] private Vector3 normalOffset = new Vector3(0, 1.5f, 0);
    [SerializeField] private Vector3 zoomOffset = new Vector3(1, 1, 0);
    [SerializeField] private float yRotationMin = -10f;
    [SerializeField] private float yRotationMax = 80f;
    [SerializeField] private float smoothTime = 0.12f;
    [SerializeField] private float zoomSpeed = 10;
    [SerializeField] private LayerMask collisionMask;
    private float rotationX;
    private float distance;
    private Vector3 offset;
    private Vector3 targetRotation;
    private Vector3 currentVel;
    public bool IsRunning { get; set; } = false;

    private void Awake() {
        rotationX = normalRotationX;
        distance = normalDistance;
        offset = normalOffset;
    }

    public void Run() {
        IsRunning = true;
    }
    
    private void LateUpdate() {
        if(IsRunning == false)
            return;
        xAxis += Input.GetAxis("Mouse X")*rotSensitive;
        yAxis -= Input.GetAxis("Mouse Y")*rotSensitive;
        yAxis = Mathf.Clamp(yAxis, yRotationMin, yRotationMax);
        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(yAxis + rotationX, xAxis), ref currentVel, smoothTime);
        transform.eulerAngles = targetRotation;
        Vector3 desiredPosition = target.position - transform.forward * distance + transform.rotation * offset;

        Vector3 collisionDetectionDir = desiredPosition - target.position;
        Ray ray = new Ray(target.position, collisionDetectionDir);
        Debug.DrawRay(ray.origin, ray.direction * collisionDetectionDir.magnitude, Color.red, 1);
        if(Physics.Raycast(ray, out RaycastHit hit, collisionDetectionDir.magnitude, collisionMask)) {
            desiredPosition = hit.point;
        }
        transform.position = desiredPosition;
    }

    public void ZoomIn() {
        rotationX = Mathf.Lerp(rotationX, zoomRotationX, zoomSpeed * Time.deltaTime);
        distance = Mathf.Lerp(distance, zoomDistance, zoomSpeed * Time.deltaTime);
        offset = Vector3.Lerp(offset, zoomOffset, zoomSpeed * Time.deltaTime);
    }

    public void ZoomOut() {
        rotationX = Mathf.Lerp(rotationX, normalRotationX, zoomSpeed * Time.deltaTime);
        distance = Mathf.Lerp(distance, normalDistance, zoomSpeed * Time.deltaTime);
        offset = Vector3.Lerp(offset, normalOffset, zoomSpeed * Time.deltaTime);
    }

    public Ray GetScreenCenterRay() {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        return GetComponent<Camera>().ScreenPointToRay(screenCenter);
    }

    public Vector3 GetScreenCenterPoint() {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        return GetComponent<Camera>().ScreenToWorldPoint(screenCenter);
    }

    public void RecoilUpDown() {
        StartCoroutine(KeepingTopBotRetro());
    }

    private IEnumerator KeepRecoilingUpDown() {
        //const float RecoilOffset = 50f;
        const float RecoilPower = 10000f;
        float recoildDuration = 0.03f;
        while(recoildDuration > 0) {
            yAxis -= RecoilPower * Time.deltaTime * recoildDuration;
            recoildDuration -= Time.deltaTime;
            yield return null;
        }
        recoildDuration = 0.03f;
        while(recoildDuration > 0) {
            yAxis += RecoilPower * Time.deltaTime * recoildDuration;
            recoildDuration -= Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator KeepingTopBotRetro() {
        const float RetroPower = 3f;
        float rTime = 0;
        float retroRotX = transform.localEulerAngles.x - RetroPower;
        float retroOriginRotX = transform.localEulerAngles.x;
        Vector3 retroPower = new Vector3(RetroPower, 0f, 0f);
        while(RotConver(transform.localEulerAngles.x) - 0.05f >= RotConver(retroRotX)) {
            if(transform.localEulerAngles.x > 289 && transform.localEulerAngles.x < 290)
                break;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(transform.localEulerAngles - retroPower), 150f * Time.deltaTime);
            yAxis = RotConver(transform.localEulerAngles.x);
            rTime += Time.deltaTime;
            if(rTime > 0.2f) {
                rTime = 0;
                break;
            }
            yield return null;
        }
        while(RotConver(transform.localEulerAngles.x) + 0.1f <= RotConver(retroOriginRotX)) {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(transform.localEulerAngles + retroPower), 25f * Time.deltaTime);
            yAxis = RotConver(transform.localEulerAngles.x);
            rTime += Time.deltaTime;
            if(rTime > 0.4f) {
                rTime = 0;
                break;
            }
            yield return null;
        }
    }

    private float RotConver(float _rot) {
        if(_rot > 180)
            _rot -= 360;
        return _rot;
    }
}
