using UnityEngine;

public static class RamerDouglasPeucker
{
    public static float DistanceSquared(Vector3[] vertices, int start, int end, int current)
    {
        float a = vertices[current].x - vertices[start].x;
        float b = vertices[current].z - vertices[start].z;
        float c = vertices[end].x - vertices[start].x;
        float d = vertices[end].z - vertices[start].z;

        float u = -1f;
        float dot = a * c + b * d;
        float lengthSquared = c * c + d * d;
        if (lengthSquared > 0f) { u = dot / lengthSquared; }
        float projectedX, projectedY;
        if (u < 0f)
        {
            projectedX = vertices[start].x;
            projectedY = vertices[start].z;
        }
        else if (u > 1f)
        {
            projectedX = vertices[end].x;
            projectedY = vertices[end].z;
        }
        else
        {
            projectedX = vertices[start].x + u * c;
            projectedY = vertices[start].z + u * d;
        }
        float dx = vertices[current].x - projectedX;
        float dy = vertices[current].z - projectedY;
        return dx * dx + dy * dy;
    }

    public static void SimplifyPartial(Vector3[] vertices, bool[] isRemoved, float thresholdSquared, int start, int end)
    {
        if (end - start < 2) { return; }
        int startIndex = start % vertices.Length;
        int endIndex = end % vertices.Length;
        float maxDistance = 0f;
        int maxIndex = start + 1;
        for (int i = start + 1; i < end; ++i)
        {
            int index = i % vertices.Length;
            float distance = DistanceSquared(vertices, startIndex, endIndex, index);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                maxIndex = i;
            }
        }
        if (maxDistance > thresholdSquared)
        {
            SimplifyPartial(vertices, isRemoved, thresholdSquared, start, maxIndex);
            SimplifyPartial(vertices, isRemoved, thresholdSquared, maxIndex, end);
        }
        else
        {
            for (int i = start + 1; i < end; ++i)
            {
                int index = i % vertices.Length;
                isRemoved[index] = true;
            }
        }
    }
}
