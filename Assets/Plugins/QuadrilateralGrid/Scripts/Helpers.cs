using UnityEngine;

public static class Helpers
{
    public static bool InRange(float value, Vector2 range)
    {
        return value >= range.x && value <= range.y;
    }
}