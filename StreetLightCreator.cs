using UnityEngine;
using System.Collections.Generic;


public class StreetLightCreator : MonoBehaviour {
    [SerializeField] [Range(0, 1f)] private float createProbaility;

    private void Start() {
        List<Transform> streetLightTransforms = FindChildrenByName(transform, "StreetLightCollider (6)");
        foreach (Transform streetLightTransform in streetLightTransforms) {
            CreatePointLight(streetLightTransform);
        }
        if (streetLightTransforms.Count == 0) {
            Debug.LogError("No StreetLightCollider (6) objects found!");
        }
    }

    private List<Transform> FindChildrenByName(Transform parent, string name) {
        List<Transform> result = new List<Transform>();
        foreach (Transform child in parent) {
            if (child.name == name) {
                result.Add(child);
            }
            result.AddRange(FindChildrenByName(child, name));
        }
        return result;
    }

    private void CreatePointLight(Transform lamp) {
        if(Random.value >= createProbaility)
            return;
        GameObject lightGameObject = new GameObject("Point Light");
        lightGameObject.transform.SetParent(lamp);
        Light pointLight = lightGameObject.AddComponent<Light>();
        pointLight.type = LightType.Point;
        lightGameObject.transform.position = lamp.position;
        // Optionally set light properties
        pointLight.lightmapBakeType = LightmapBakeType.Baked;
        pointLight.range = 10f;
        pointLight.intensity = 1f;
        pointLight.color = Color.white;
    }
}
