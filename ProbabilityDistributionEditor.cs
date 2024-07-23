using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProbabilityDistribution))]
public class ProbabilityDistributionEditor : Editor
{
    private SerializedProperty probabilityCurveProperty;

    private void OnEnable()
    {
        probabilityCurveProperty = serializedObject.FindProperty("feelingProbabilityCurve");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(probabilityCurveProperty);

        if (GUILayout.Button("Ensure Curve Endpoints and Non-Negative Values"))
        {
            var distribution = (ProbabilityDistribution)target;
            distribution.EnsureCurveEndpoints();
            distribution.EnsureCurveNonNegative();
        }

        serializedObject.ApplyModifiedProperties();
    }
}