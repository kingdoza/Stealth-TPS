using System;
using UnityEngine;

public class LightInterpolator : MonoBehaviour {
    [SerializeField] private Light[] lights;
    private float maxDistance = 18;

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Player>() != null) {
            //maxDistance = Vector3.Distance(other.transform.position, transform.position);
            //Debug.Log(maxDistance);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.GetComponent<Player>() != null) {
            float brightness = GetBrightnessFromDistance(other);
            UpdateLightIntensity(brightness);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<Player>() == null)
            return;
        if(lights[0].intensity > 0.5f) {
            UpdateLightIntensity(1);
        }
        else {
            UpdateLightIntensity(0);
        }
    }

    private float GetBrightnessFromDistance(Collider other) {
        float distance = Vector3.Distance(other.transform.position, transform.position);
        return 1f - distance / maxDistance;
    }

    private void UpdateLightIntensity(float brightness) {
        for(int i = 0; i < lights.Length; ++i) {
            lights[i].intensity = Mathf.Lerp(0, 1, brightness);
        }
    }
}
