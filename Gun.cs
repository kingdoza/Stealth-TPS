using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
 
[RequireComponent(typeof(LineRenderer))]
public class Gun : MonoBehaviour {
    [SerializeField] private AudioClip[] fireEffects;
    private AudioSource audioSource;
    private bool isSighting = false;
    private LineRenderer laserLine;
    private float gunRange = 50f;
    [SerializeField] private float fireDelay = 0.07f;
    protected float remainedFireDelay;
    //private float originAngleX;
    [SerializeField] private LayerMask hittableMask;
    [SerializeField] private GameObject laserPoint;
    [SerializeField] [Range(0, 360)] private float nonSightedAngle;
    [SerializeField] private BulletHolePool bulletHolePool;
    //private CameraController3D cameraController;
 
    protected virtual void Awake() {
        laserPoint.SetActive(false);
        //originAngleX = transform.rotation.eulerAngles.x;
        remainedFireDelay = fireDelay;
        laserLine = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>(); 
        //cameraController = Camera.main.GetComponent<CameraController3D>();
        CancelSight();
    }

    protected virtual void Update() {
        remainedFireDelay -= Time.deltaTime;
        if(remainedFireDelay <= 0)
            remainedFireDelay = 0f;

        SetLaserLineWhileSighting();
    }

    private void SetLaserLineWhileSighting() {
        /*
        if(isSighting) {
            laserLine.SetPosition(0, transform.position);
            laserLine.SetPosition(1, transform.position + -transform.forward * gunRange);
        } */
        if(!isSighting)
            return;
        SetLaserPointPos(out Ray ray);
        if (Physics.Raycast(transform.position, ray.direction, out RaycastHit hit, gunRange, hittableMask)) {
            //laserPoint.transform.position = hit.point;    //3D레이저포인터
            laserPoint.transform.position = hit.point + hit.normal * 0.015f;
            laserPoint.transform.rotation = Quaternion.LookRotation(hit.normal);
            laserPoint.SetActive(true);
            laserLine.SetPosition(1, hit.point);
        }
        else {
            laserPoint.SetActive(false);
            laserLine.SetPosition(1, transform.position + ray.direction * gunRange);
        }
    }

    private void SetLaserPointPos(out Ray ray) {
        laserLine.SetPosition(0, transform.position);
        ray = new Ray(transform.position, transform.forward * gunRange);
        laserLine.SetPosition(1, transform.position + ray.direction * gunRange);
    }

    public void SightAfterDelay(float delay) {
        Invoke("Sight", delay);
    }

    public void CalcelSightAfterDelay(float delay) {
        Invoke("CancelSight", delay);
    }

    public void Sight() {
        SetLaserPointPos(out Ray laserRay);
        isSighting = true;
        laserLine.enabled = true;
    }

    public void CancelSight() {
        isSighting = false;
        laserPoint.SetActive(false);
        laserLine.enabled = false;
    }

    public virtual void PullTrigger() {
        if(remainedFireDelay <= Mathf.Epsilon) {
            Fire();
        }
    }

    protected virtual void Fire() {
        remainedFireDelay = fireDelay;
        int randomIndex = Random.Range(0, fireEffects.Length);
        audioSource.clip = fireEffects[randomIndex];
        GameManager.Instance.soundManager.PlayAudioSource(audioSource);
        ShootRaycast();
        //cameraController.RecoilUpDown();
        //StartCoroutine(RecoilUpDown());
    }

    private void ShootRaycast() {
        Vector3 shootingDir;
        if(isSighting)
            //shootingDir = cameraController.GetScreenCenterRay().direction;
            shootingDir = transform.forward;
        else {
            shootingDir = GetRandomDirectionWithAngle();
        }
        //Ray ray = cameraController.GetScreenCenterRay();
        Ray ray = new Ray(transform.position, shootingDir);
        RaycastHit[] hits = Physics.RaycastAll(ray, gunRange, hittableMask);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        if(hits.Length >= 2 && hits[0].collider.GetComponent<Agent>() != null) {
            bulletHolePool.MakeBulletHole(hits[0], ray);
            bulletHolePool.MakeBulletHole(hits[1], ray);
        } else if(hits.Length >= 1) {
            bulletHolePool.MakeBulletHole(hits[0], ray);
        }
        Debug.DrawRay(transform.position, shootingDir * gunRange, Color.red, 1f);
    }

    private Vector3 GetRandomDirectionWithAngle() {
        float halfAngle = nonSightedAngle / 2f;
        float randomAngle = Random.Range(-halfAngle, halfAngle);
        Quaternion rotation = Quaternion.AngleAxis(randomAngle, transform.up);
        Vector3 direction = rotation * transform.forward;
        return direction.normalized;
    }

/*
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
    } */
}
