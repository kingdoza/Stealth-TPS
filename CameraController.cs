using TreeEditor;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private Transform player;
    private float rotateSmooth = 100f;
    private float zoomSpeed = 15f;

    private Vector3 normalOffset;
    private Vector3 zoomInOffset;
    private Vector3 offset;
    private float normalAngleX;
    private float zoomInAngleX;
    private float angleX;


    private void Awake() {
        normalAngleX = transform.rotation.eulerAngles.x;
        zoomInAngleX = normalAngleX - 10f;
        angleX = normalAngleX;

        normalOffset = transform.position - player.position;
        zoomInOffset = normalOffset + new Vector3(0, -1f, 1f);
        offset = normalOffset;
    }

    private void LateUpdate() {
        Vector3 targetPosition = player.position + player.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, rotateSmooth * Time.deltaTime);
        Vector3 targetRotation = new Vector3(angleX, player.rotation.eulerAngles.y);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), rotateSmooth * Time.deltaTime);
    }

    public void ZoomIn() {
        angleX = Mathf.Lerp(angleX, zoomInAngleX, zoomSpeed * Time.deltaTime);
        offset = Vector3.Lerp(offset, zoomInOffset, zoomSpeed * Time.deltaTime);
    }

    public void ZoomOut() {
        angleX = Mathf.Lerp(angleX, normalAngleX, zoomSpeed * Time.deltaTime);
        offset = Vector3.Lerp(offset, normalOffset, zoomSpeed * Time.deltaTime);
    }
}
