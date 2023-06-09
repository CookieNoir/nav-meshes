using System.Collections.Generic;
using UnityEngine;

public static class EarClippingTriangulation
{
    public class VertexNode
    {
        public Vector3 Position { get; private set; }
        public VertexNode Previous;
        public VertexNode Next;

        public VertexNode(Vector3 position)
        {
            Position = position;
        }
    }

    public static List<NavMeshCell> GetTriangles(NavMeshPolygon polygon)
    {
        return GetTriangles(polygon.Vertices);
    }

    public static List<NavMeshCell> GetTriangles(Vector3[] vertices)
    {
        List<NavMeshCell> cells = new List<NavMeshCell>();
        if (vertices.Length < 3) { return cells; }
        else if (vertices.Length == 3)
        {
            cells.Add(new NavMeshCell(new Vector3[] { vertices[0], vertices[1], vertices[2] }));
            return cells;
        }
        VertexNode firstNode = new VertexNode(vertices[0]);
        VertexNode previousNode = firstNode;
        for (int i = 1; i < vertices.Length; ++i)
        {
            VertexNode node = new VertexNode(vertices[i]);
            node.Previous = previousNode;
            previousNode.Next = node;
            previousNode = node;
        }
        previousNode.Next = firstNode;
        firstNode.Previous = previousNode;

        Queue<VertexNode> ears = new Queue<VertexNode>();
        int verticesCount = vertices.Length;

        bool IsEar(VertexNode node)
        {
            VertexNode previous = node.Previous;
            VertexNode next = node.Next;
            bool isEar = true;
            VertexNode outerVertex = next.Next;
            do
            {
                if (IsPointInTriangle(outerVertex.Position, previous.Position, node.Position, next.Position))
                {
                    isEar = false;
                    break;
                }
                outerVertex = outerVertex.Next;
            }
            while (outerVertex != node.Previous);
            return isEar;
        }

        void TryAddEar(VertexNode node)
        {
            VertexNode previous = node.Previous;
            VertexNode next = node.Next;
            if (Sign(previous.Position, next.Position, node.Position) < 0f)
            {
                if (IsEar(node)) { ears.Enqueue(node); }
            }
        }

        VertexNode workingNode = firstNode;
        do
        {
            TryAddEar(workingNode);
            workingNode = workingNode.Next;
        }
        while (workingNode != firstNode);



        while (ears.TryDequeue(out VertexNode ear))
        {
            if (!IsEar(ear)) { continue; }
            VertexNode previous = ear.Previous;
            VertexNode next = ear.Next;
            cells.Add(new NavMeshCell(new Vector3[] { previous.Position, ear.Position, next.Position }));
            previous.Next = next;
            next.Previous = previous;
            if (!ears.Contains(previous)) { TryAddEar(previous); }
            if (!ears.Contains(next)) { TryAddEar(next); }
            verticesCount--;
            if (verticesCount == 3) { break; }
        }
        VertexNode v1 = ears.Dequeue();
        VertexNode v2 = v1.Previous;
        VertexNode v3 = v1.Next;
        cells.Add(new NavMeshCell(new Vector3[] { v1.Position, v2.Position, v3.Position }));
        return cells;
    }

    private static float Sign(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        return (v2.x - v1.x) * (v3.z - v1.z) - (v3.x - v1.x) * (v2.z - v1.z);
    }

    private static bool IsPointInTriangle(Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float s1 = Sign(v1, v2, point),
              s2 = Sign(v2, v3, point),
              s3 = Sign(v3, v1, point);
        bool hasNegative = (s1 < 0) || (s2 < 0) || (s3 < 0),
             hasPositive = (s1 > 0) || (s2 > 0) || (s3 > 0);
        return !(hasNegative && hasPositive);
    }
}
