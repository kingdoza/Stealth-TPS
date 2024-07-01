using System.Collections;
using UnityEngine;

public class PlayerGun : Gun {
    private CameraController3D cameraController;
    private float originAngleX;
    private int currentBulletInMagazin = 0;
    [SerializeField] private int maxBulletsInMagazin;
    [SerializeField] private int totalBullets;

    protected override void Awake() {
        base.Awake();
        originAngleX = transform.rotation.eulerAngles.x;
        cameraController = Camera.main.GetComponent<CameraController3D>();
    }

    private void Start() {
        Reload();
    }

    public override void PullTrigger() {
        if(currentBulletInMagazin <= 0) {
            Reload();
            return;
        }
        base.PullTrigger();
    }

    protected override void Fire() {
        base.Fire();
        currentBulletInMagazin--;
        GameManager.Instance.uiManager.gunInfoUI.UpdateBulletUI(currentBulletInMagazin, totalBullets);
        if(currentBulletInMagazin <= 0) {
            Reload();
        }
        StartCoroutine(RecoilUpDown());
    }

    private void Reload() {
        if(maxBulletsInMagazin <= 0)
            return;
        int bulletsToReload = (totalBullets < maxBulletsInMagazin) ? totalBullets : maxBulletsInMagazin;
        totalBullets -= bulletsToReload;
        currentBulletInMagazin += bulletsToReload;
        GameManager.Instance.uiManager.gunInfoUI.UpdateBulletUI(currentBulletInMagazin, totalBullets);
    }

    private IEnumerator RecoilUpDown() {
        const float RecoilOffset = 2f;
        const float RecoilPower = 120f;
        float currentAngleX = transform.localRotation.eulerAngles.x;
        Vector3 targetRotation = transform.localRotation.eulerAngles;
        targetRotation.x = originAngleX + RecoilOffset;
        while(currentAngleX <= originAngleX + RecoilOffset - 0.01f) {
            Vector3 cameraRot = Camera.main.transform.localRotation.eulerAngles;
            cameraRot.x += RecoilOffset / 2f;
            Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, Quaternion.Euler(cameraRot), RecoilPower * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetRotation), RecoilPower * Time.deltaTime);
            currentAngleX = transform.localRotation.eulerAngles.x;
            yield return null;
        }
        targetRotation.x = originAngleX;
        while(currentAngleX >= originAngleX + 0.01f) {
            Vector3 cameraRot = Camera.main.transform.localRotation.eulerAngles;
            cameraRot.x -= RecoilOffset / 2f;
            Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, Quaternion.Euler(cameraRot), RecoilPower * Time.deltaTime / 2);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetRotation), RecoilPower * Time.deltaTime / 2);
            currentAngleX = transform.localRotation.eulerAngles.x;
            yield return null;
        }
        transform.localRotation = Quaternion.Euler(targetRotation);
    }
}
