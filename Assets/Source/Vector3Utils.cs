using UnityEngine;

public static class Vector3Utils
{
    public static bool IsPointInTriangle(Vector2 point, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float s1 = Sign(point, v1, v2),
              s2 = Sign(point, v2, v3),
              s3 = Sign(point, v3, v1);
        bool hasNegative = (s1 < 0) || (s2 < 0) || (s3 < 0),
             hasPositive = (s1 > 0) || (s2 > 0) || (s3 > 0);
        return !(hasNegative && hasPositive);
    }

    public static bool IsPointInCone(Vector2 point, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float s1 = Sign(point, v1, v2),
              s2 = Sign(point, v2, v3);
        return s1 > 0 && s2 > 0;
    }

    private static float Sign(Vector2 p1, Vector3 p2, Vector3 p3)
    {
        return (p1.x - p3.x) * (p2.z - p3.z) - (p2.x - p3.x) * (p1.y - p3.z);
    }
}
