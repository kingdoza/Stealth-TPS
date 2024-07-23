using System.Collections.Generic;
using UnityEngine;

public class ProbabilityDistribution : MonoBehaviour
{
    public AnimationCurve feelingProbabilityCurve = AnimationCurve.Linear(0, 0, 1, 1);
    private AnimationCurve cdfCurve;
    public AnimationCurve pdfCurve;

    private void Start() {
        cdfCurve = GenerateCDF(feelingProbabilityCurve);
        for(int i = 0; i < cdfCurve.keys.Length; ++i) {
            //Debug.Log("cdfCurve.keys[" + i + "].time : " + cdfCurve.keys[i].time);
            //Debug.Log("cdfCurve.keys[" + i + "].value : " + cdfCurve.keys[i].value);
        }
    }

    public void EnsureCurveEndpoints()
    {
        if (feelingProbabilityCurve == null)
            feelingProbabilityCurve = new AnimationCurve();

        var keys = feelingProbabilityCurve.keys;

        if (keys.Length == 0)
        {
            feelingProbabilityCurve.AddKey(0, 0);
            feelingProbabilityCurve.AddKey(1, 1);
        }
        else
        {
            if (keys[0].time != 0)
                feelingProbabilityCurve.MoveKey(0, new Keyframe(0, Mathf.Max(keys[0].value, 0)));

            if (keys[keys.Length - 1].time != 1)
                feelingProbabilityCurve.MoveKey(keys.Length - 1, new Keyframe(1, Mathf.Max(keys[keys.Length - 1].value, 0)));
        }
    }

    public void EnsureCurveNonNegative()
    {
        if (feelingProbabilityCurve == null)
            return;

        var keys = feelingProbabilityCurve.keys;
        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i].value < 0)
                keys[i].value = 0;
        }
        feelingProbabilityCurve.keys = keys;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Result : " + GenerateRandomValue());
        }
    }

    private AnimationCurve GenerateCDF(AnimationCurve pdf)
    {
        AnimationCurve cdf = new AnimationCurve();
        float stepSize = 0.01f;
        List<float> areas = new List<float>();
        areas.Add(pdf.Evaluate(0) * stepSize);
        float previousTotalArea = areas[0];

        for (float t = stepSize; t <= 1; t += stepSize)
        {
            float currentArea = pdf.Evaluate(t) * stepSize;
            currentArea += previousTotalArea;
            areas.Add(currentArea);
            previousTotalArea = currentArea;
        }

        // Normalize CDF to [0, 1]
        float totalArea = areas[areas.Count - 1];
        for (int i = 0; i < areas.Count; ++i)
        {
            cdf.AddKey(new Keyframe(stepSize * i, areas[i] / totalArea));
            //Debug.Log("cdf.keys[" + i + "].value" + cdf.keys[i].value);
        }

        return cdf;
    }

    private void OnValidate()
    {
        EnsureCurveEndpoints();
        EnsureCurveNonNegative();
        cdfCurve = GenerateCDF(feelingProbabilityCurve); // CDF 업데이트
    }

    public float GenerateRandomValue()
    {
        float u = Random.value;

        for (int i = 1; i < cdfCurve.keys.Length; i++)
        {
            //Debug.Log("cdfCurve.keys[" + i + "].value : " + cdfCurve.keys[i].value);
            if (cdfCurve.keys[i].value >= u)
            {
                float t0 = cdfCurve.keys[i - 1].time;
                float t1 = cdfCurve.keys[i].time;
                float v0 = cdfCurve.keys[i - 1].value;
                float v1 = cdfCurve.keys[i].value;
                //Debug.Log("t0 : " + t0);
                //Debug.Log("t1 : " + t1);
                //Debug.Log("v0 : " + v0);
                //Debug.Log("v1 : " + v1);

                return Mathf.Lerp(t0, t1, (u - v0) / (v1 - v0));
            }
        }
        return 1.0f; // Shouldn't reach here if CDF is well-formed
    }
    /*

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Result : " + GenerateRandomValue());
        }
    }

    private void OnValidate()
    {
        EnsureCurveEndpoints();
        EnsureCurveNonNegative();
        cdfCurve = GenerateCDF(feelingProbabilityCurve); // CDF 업데이트
    }

    public void EnsureCurveEndpoints()
    {
        if (feelingProbabilityCurve == null)
            feelingProbabilityCurve = new AnimationCurve();

        var keys = feelingProbabilityCurve.keys;

        if (keys.Length == 0)
        {
            feelingProbabilityCurve.AddKey(0, 0);
            feelingProbabilityCurve.AddKey(1, 1);
        }
        else
        {
            if (keys[0].time != 0)
                feelingProbabilityCurve.MoveKey(0, new Keyframe(0, Mathf.Max(keys[0].value, 0)));

            if (keys[keys.Length - 1].time != 1)
                feelingProbabilityCurve.MoveKey(keys.Length - 1, new Keyframe(1, Mathf.Max(keys[keys.Length - 1].value, 0)));
        }
    }

    public void EnsureCurveNonNegative()
    {
        if (feelingProbabilityCurve == null)
            return;

        var keys = feelingProbabilityCurve.keys;
        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i].value < 0)
                keys[i].value = 0;
        }
        feelingProbabilityCurve.keys = keys;
    }

    private AnimationCurve GenerateCDF(AnimationCurve pdf)
    {
        AnimationCurve cdf = new AnimationCurve();
        float totalArea = 0;
        float stepSize = 0.01f;

        for (float t = 0; t <= 1; t += stepSize)
        {
            totalArea += pdf.Evaluate(t) * stepSize;
            cdf.AddKey(new Keyframe(t, totalArea));
        }

        // Normalize CDF to [0, 1]
        for (int i = 0; i < cdf.keys.Length; i++)
        {
            cdf.keys[i].value /= totalArea;
        }

        return cdf;
    }

    public float GetProbability(float x)
    {
        return feelingProbabilityCurve.Evaluate(x);
    }

    public float GenerateRandomValue()
    {
        float u = Random.value;
        Debug.Log("RandomValue : " + u);

        for (int i = 1; i < cdfCurve.keys.Length; i++)
        {
            Debug.Log("cdfCurve.keys[" + i + "].value : " + cdfCurve.keys[i].value);
            if (cdfCurve.keys[i].value >= u)
            {
                float t0 = cdfCurve.keys[i - 1].time;
                float t1 = cdfCurve.keys[i].time;
                float v0 = cdfCurve.keys[i - 1].value;
                float v1 = cdfCurve.keys[i].value;
                Debug.Log("t0 : " + t0);
                Debug.Log("t1 : " + t1);
                Debug.Log("v0 : " + v0);
                Debug.Log("v1 : " + v1);

                return Mathf.Lerp(t0, t1, (u - v0) / (v1 - v0));
            }
        }

        return 1.0f; // Shouldn't reach here if CDF is well-formed
    } */
}