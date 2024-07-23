using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGun : Gun, IStageRun {
    private CameraController3D cameraController;
    private float originAngleX;
    private int currentBulletInMagazin = 0;
    [SerializeField] private int maxBulletsInMagazin;
    [SerializeField] private int totalBullets;
    public int TotalBullets => totalBullets;
    public bool IsRunning { get; set; } = false;

    protected override void Awake() {
        base.Awake();
        originAngleX = transform.rotation.eulerAngles.x;
        cameraController = Camera.main.GetComponent<CameraController3D>();
    }

    public void Run(){
        IsRunning = true;
        bulletHolePool = FindObjectOfType<BulletHolePool>();
    }

    private void Start() {
        Reload();
    }

    public override void PullTrigger() {
        if(IsRunning == false)
            return;
        if(currentBulletInMagazin <= 0)
            return;
        base.PullTrigger();
    }

    protected override void Fire() {
        base.Fire();
        currentBulletInMagazin--;
        GameManager.Instance.uiManager.gunInfoUI.UpdateBulletUI(currentBulletInMagazin, totalBullets);
        //if(currentBulletInMagazin <= 0) {
            //Reload();
        //}
        StartCoroutine(RecoilUpDown());
    }

    public bool CanReload() {
        return currentBulletInMagazin < maxBulletsInMagazin && totalBullets > 0;
    }

    public bool IsMagazinEmpty() {
        return currentBulletInMagazin <= 0;
    }

    public void Reload() {
        if(maxBulletsInMagazin <= 0)
            return;
        int bulletsToReload = maxBulletsInMagazin - currentBulletInMagazin;
        if(bulletsToReload > totalBullets)
            bulletsToReload = totalBullets;
        totalBullets -= bulletsToReload;
        currentBulletInMagazin += bulletsToReload;
        GameManager.Instance.uiManager.gunInfoUI.UpdateBulletUI(currentBulletInMagazin, totalBullets);
    }

    private IEnumerator RecoilUpDown() {
        Debug.Log("recoild");
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

    protected override RaycastHit[] GetBulletRayHits() {
        if(isSighting)
            //shootingDir = cameraController.GetScreenCenterRay().direction;
            SetLaserPointPos(out shootRay);
        else {
            Vector3 shootingDir = GetRandomDirectionWithAngle();
            shootRay = new Ray(muzzle.position, shootingDir * gunRange);
        }
        //Ray ray = cameraController.GetScreenCenterRay();
        List<RaycastHit> hits = Physics.RaycastAll(shootRay, gunRange, hittableMask).ToList();
        hits.Sort((a, b) => a.distance.CompareTo(b.distance));
        if(hits.Count >= 2 && hits[0].collider.GetComponent<Agent>() != null) {
            hits.RemoveRange(2, hits.Count - 2);
        } else if(hits.Count >= 1) {
            hits.RemoveRange(1, hits.Count - 1);
        }
        Debug.DrawRay(muzzle.position, shootRay.direction * gunRange, Color.red, 1f);
        return hits.ToArray();
    }

    protected override void HitTargets(RaycastHit[] hits, Ray ray) {
        foreach(RaycastHit hit in hits) {
            bulletHolePool.MakeBulletHole(hit, ray);
            Agent agentHit = hit.collider.GetComponent<Enemy>();
            if(agentHit == null)
                return;
            agentHit.TakeHit(damage);
        }
    }
}
