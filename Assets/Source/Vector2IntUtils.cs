using UnityEngine;

public static class Vector2IntUtils
{
    public static bool IsPointInTriangle(int x, int y, Vector2Int p1, Vector2Int p2, Vector2Int p3)
    {
        int s1 = SignInt(x, y, p1, p2),
            s2 = SignInt(x, y, p2, p3),
            s3 = SignInt(x, y, p3, p1);
        bool hasNegative = (s1 < 0) || (s2 < 0) || (s3 < 0),
             hasPositive = (s1 > 0) || (s2 > 0) || (s3 > 0);
        return !(hasNegative && hasPositive);
    }

    public static bool IsPointInCone(Vector2Int point, Vector2Int p1, Vector2Int p2, Vector2Int p3)
    {
        int s1 = SignInt(point.x, point.y, p1, p2);
        int s2 = SignInt(point.x, point.y, p2, p3);
        return s1 > 0 && s2 > 0;
    }

    private static int SignInt(int p1x, int p1y, Vector2Int p2, Vector2Int p3)
    {
        return (p1x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1y - p3.y);
    }
}
