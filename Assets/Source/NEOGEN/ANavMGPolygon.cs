using System.Collections.Generic;
using UnityEngine;

public class ANavMGPolygon
{
    public Vector2Int[] Vertices { get; private set; }

    public ANavMGPolygon(List<Vector2Int> vertices)
    {
        Vertices = vertices.ToArray();
    }

    public void Simplify(float threshold)
    {
        bool[] isRemoved = new bool[Vertices.Length];
        int survivedCount = Vertices.Length;
        float thresholdSquared = threshold * threshold;

        float DistanceSquared(int start, int end, int current)
        {
            float a = Vertices[current].x - Vertices[start].x;
            float b = Vertices[current].y - Vertices[start].y;
            float c = Vertices[end].x - Vertices[start].x;
            float d = Vertices[end].y - Vertices[start].y;

            float u = -1f;
            float dot = a * c + b * d;
            float lengthSquared = c * c + d * d;
            if (lengthSquared > 0f) { u = dot / lengthSquared; }
            float projectedX, projectedY;
            if (u < 0f)
            {
                projectedX = Vertices[start].x;
                projectedY = Vertices[start].y;
            }
            else if (u > 1f)
            {
                projectedX = Vertices[end].x;
                projectedY = Vertices[end].y;
            }
            else
            {
                projectedX = Vertices[start].x + u * c;
                projectedY = Vertices[start].y + u * d;
            }
            float dx = Vertices[current].x - projectedX;
            float dy = Vertices[current].y - projectedY;
            return dx * dx + dy * dy;
        }

        void RdpPartial(int start, int end)
        {
            if (end - start < 2) { return; }
            int endIndex = end % Vertices.Length;
            float maxDistance = 0f;
            int maxIndex = start + 1;
            for (int i = start + 1; i < end; ++i)
            {
                float distance = DistanceSquared(start, endIndex, i);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxIndex = i;
                }
            }
            if (maxDistance > thresholdSquared)
            {
                RdpPartial(start, maxIndex);
                RdpPartial(maxIndex, end);
            }
            else
            {
                for (int i = start + 1; i < end; ++i)
                {
                    isRemoved[i] = true;
                    survivedCount--;
                }
            }
        }

        int midPoint = Vertices.Length / 2;
        RdpPartial(0, midPoint);
        RdpPartial(midPoint, Vertices.Length);

        Vector2Int[] newVertices = new Vector2Int[survivedCount];
        int index = 0;
        for (int i = 0; i < Vertices.Length; ++i)
        {
            if (!isRemoved[i])
            {
                newVertices[index] = Vertices[i];
                index++;
            }
        }
        Vertices = newVertices;
    }
}
