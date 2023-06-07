using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Polygon
{
    [SerializeField] public Vector3[] Vertices { get; private set; }

    public Polygon(List<Vector3> vertices)
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
            float b = Vertices[current].z - Vertices[start].z;
            float c = Vertices[end].x - Vertices[start].x;
            float d = Vertices[end].z - Vertices[start].z;

            float u = -1f;
            float dot = a * c + b * d;
            float lengthSquared = c * c + d * d;
            if (lengthSquared > 0f) { u = dot / lengthSquared; }
            float projectedX, projectedY;
            if (u < 0f)
            {
                projectedX = Vertices[start].x;
                projectedY = Vertices[start].z;
            }
            else if (u > 1f)
            {
                projectedX = Vertices[end].x;
                projectedY = Vertices[end].z;
            }
            else
            {
                projectedX = Vertices[start].x + u * c;
                projectedY = Vertices[start].z + u * d;
            }
            float dx = Vertices[current].x - projectedX;
            float dy = Vertices[current].z - projectedY;
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

        Vector3[] newVertices = new Vector3[survivedCount];
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
