using System;
using UnityEngine;

public enum Gender {

    Male, Female
}

public enum Sensitivity {
    Low, Medium, High
}

public static class EnumExtensions {
    public static string ToLocalizedString(this Gender gender) {
        string toString = gender switch {
            Gender.Male => "남성",
            Gender.Female => "여성",
            _ => throw new ArgumentOutOfRangeException(nameof(gender), gender, null)
        };
        return toString;
    }

    public static string ToLocalizedString(this Sensitivity sensitivity) {
        string toString = sensitivity switch {
            Sensitivity.Low => "하",
            Sensitivity.Medium => "중",
            Sensitivity.High => "상",
            _ => throw new ArgumentOutOfRangeException(nameof(sensitivity), sensitivity, null)
        };
        return toString;
    }

    public static Color ToLocalizedColor(this Sensitivity sensitivity) {
        Color toColor = sensitivity switch {
            Sensitivity.Low => Color.green,
            Sensitivity.Medium => Color.yellow,
            Sensitivity.High => Color.red,
            _ => throw new ArgumentOutOfRangeException(nameof(sensitivity), sensitivity, null)
        };
        return toColor;
    }

    public static T GetRandomEnumValue<T>() where T : Enum {
        Array values = Enum.GetValues(typeof(T));
        System.Random random = new System.Random();
        return (T)values.GetValue(random.Next(values.Length));
    }
}

[Serializable]
public class Client : MonoBehaviour {
    [SerializeField] private new string name;
    [SerializeField] private Gender gender;
    [SerializeField] private int age;
    [SerializeField] private string job;
    [SerializeField] private string residence;
    private Sensitivity sensitivity;
    private SleepGraph sleepGraph;
    [SerializeField] private Totem[] totems;
    [SerializeField] private string quote;
    [SerializeField] private string[] story;
    private ProbabilityDistribution feelingDistribution;

    public void Init() {
        feelingDistribution = GetComponent<ProbabilityDistribution>();
        sleepGraph = GetComponent<SleepGraph>();
        SetRandomSleepGraph();
        SetRandomSensitivity();
    }

    private void SetRandomSensitivity() {
        sensitivity = EnumExtensions.GetRandomEnumValue<Sensitivity>();
    }

    private void SetRandomSleepGraph() {
        sleepGraph.SetCycles();
    }

    public int GetRandomFeeling() {
        return (int)(feelingDistribution.GenerateRandomValue() * 100);
    }

    public string Name => name;
    public Gender Gender => gender;
    public int Age => age;
    public string Job => job;
    public string Residence => residence;
    public Sensitivity Sensitivity => sensitivity;
    public SleepGraph SleepGraph => sleepGraph;
    public Totem[] Totems => totems;
    public string Quote => quote;
    public string[] Story => story;
}
