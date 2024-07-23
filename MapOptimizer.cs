using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class MapOptimizer : MonoBehaviour {
    [SerializeField] private GameObject[] mapObjects;
    [SerializeField] private float objectActiveTimeGap = 0;
    [SerializeField] private bool isChildActivation = false;
    private bool[][] isOriginallyActive;

    private void Start() {
        SetOriginallyActiveChilds();
        objectActiveTimeGap = objectActiveTimeGap <= Mathf.Epsilon ? Time.deltaTime : objectActiveTimeGap;
        CheckPlayerInOptizingArea();
    }

    private void SetOriginallyActiveChilds() {
        if(isChildActivation == false)
            return;
        isOriginallyActive = new bool[mapObjects.Length][];
        for(int i = 0 ; i < mapObjects.Length; ++i) {
            isOriginallyActive[i] = new bool[mapObjects[i].transform.childCount];
            for(int j = 0; j < mapObjects[i].transform.childCount; ++j) {
                isOriginallyActive[i][j] = mapObjects[i].transform.GetChild(j).gameObject.activeSelf;
            }
        } 
    }

    private void CheckPlayerInOptizingArea() {
        SphereCollider collider = GetComponent<SphereCollider>();
        Vector3 sphereCenter = collider.transform.TransformPoint(collider.center);
        Debug.Log(sphereCenter + ", " + gameObject.name);
        Debug.Log(collider.radius + ", " + gameObject.name);
        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, collider.radius);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.GetComponent<Player>() != null) {
                Debug.Log("active" + gameObject.name);
                ActiveMapObjects();
                return;
            }
        }
        Debug.Log("deactive" + gameObject.name);
        DeactiveMapObjects();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Player>() != null) {
            ActiveMapObjects();
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.GetComponent<Player>() != null) {
            DeactiveMapObjects();
        }
    }

    private void ActiveMapObjects() {
        if(isChildActivation)
            StartCoroutine(ActivateChildObjectsOverTime());
        else
            StartCoroutine(ActivateMainObjectsOverTime());
        //for(int i = 0; i < mapObjects.Length; ++i) {
            //mapObjects[i].SetActive(true);
        //}
    }

    private IEnumerator ActivateMainObjectsOverTime() {
        for (int i = 0; i < mapObjects.Length; ++i) {
            mapObjects[i].SetActive(true);
            yield return new WaitForSeconds(objectActiveTimeGap); // 지정된 시간 대기
        }
    }

    private IEnumerator ActivateChildObjectsOverTime() {
        for (int i = 0; i < mapObjects.Length; ++i) {
            for(int j = 0; j < mapObjects[i].transform.childCount; ++j) {
                if(isOriginallyActive[i][j] == false)
                    continue;
                mapObjects[i].transform.GetChild(j).gameObject.SetActive(true);
                yield return new WaitForSeconds(objectActiveTimeGap);
            }
            mapObjects[i].SetActive(true);
        }
    }

    private void DeactiveMapObjects() {
        for(int i = 0; i < mapObjects.Length; ++i) {
            mapObjects[i].SetActive(false);
        }
    }
}
