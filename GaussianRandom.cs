using UnityEngine;

public static class GaussianRandom {
    public static float mean = 0.0f;
    public static float stdDev = 1.0f;
    private static System.Random random = new System.Random();

    public static float NextNumber() {
        double u1 = 1.0 - random.NextDouble();
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log((float)u1)) * Mathf.Sin(2.0f * Mathf.PI * (float)u2);
        double randNormal = mean + stdDev * randStdNormal;
        return (float)randNormal;
    }
}
