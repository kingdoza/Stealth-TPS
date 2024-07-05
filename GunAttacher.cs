using UnityEngine;

public class GunAttacher : MonoBehaviour {
    [SerializeField] private Transform weapon;  // 무기 프리팹
    [SerializeField] private Transform handle;  // 무기 프리팹
    [SerializeField] private Transform rightHand;      // 캐릭터의 오른손 본
    [SerializeField] private Vector3 weaponPositionOffset = Vector3.zero; // 무기 위치 오프셋
    [SerializeField] private Vector3 weaponRotationOffset = Vector3.zero; // 무기 회전 오프셋
    private Vector3 lastHandlePos;
    private Quaternion lastHandleRot;

    private void Awake() {
        
    }

    private void Update() {
        AttachHanle();
        //UpdateWeapon();
    }

    private void AttachHanle() {
        if (rightHand != null) {
            //weapon.transform.position = rightHand.position + rightHand.rotation * weaponPositionOffset;
            //weapon.transform.rotation = Quaternion.Euler(rightHand.rotation.eulerAngles + weaponRotationOffset);

            weapon.localPosition = weaponPositionOffset;
            weapon.localRotation = Quaternion.Euler(weaponRotationOffset);
        }
    }

    private void UpdateWeapon() {
        Vector3 currentPosition = handle.position;
        Quaternion currentRotation = handle.rotation;
        Vector3 handlePosDelta = currentPosition - lastHandlePos;
        Quaternion handleRotDelta = currentRotation * Quaternion.Inverse(lastHandleRot);
        weapon.position += handlePosDelta;
        weapon.rotation = handleRotDelta * weapon.rotation;
        lastHandlePos = currentPosition;
        lastHandleRot = currentRotation;
    }
}
