using UnityEngine;

public static class BarycentricCoordinates
{
    public static float GetDepth(Vector2 m, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float h1left = (v2.z - v3.z) * (m.x - v3.x),
              h1right = (v3.x - v2.x) * (m.y - v3.z),
              h2left = (v3.z - v1.z) * (m.x - v3.x),
              h2right = (v1.x - v3.x) * (m.y - v3.z),
              denominator = (v2.z - v3.z) * (v1.x - v3.x) + (v3.x - v2.x) * (v1.z - v3.z);
        float h1 = (h1left + h1right) / denominator;
        float h2 = (h2left + h2right) / denominator;
        float h3 = 1f - h1 - h2;
        return h1 * v1.y + h2 * v2.y + h3 * v3.y;
    }
}
