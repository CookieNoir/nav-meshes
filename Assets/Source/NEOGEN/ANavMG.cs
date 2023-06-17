using System;
using System.Collections.Generic;
using UnityEngine;

public static class ANavMG
{
    public class VertexNode: IComparable<VertexNode>
    {
        public Vector3 Position { get; private set; }
        public int RemainedVisits;
        public VertexNode Previous;
        public VertexNode Next;

        public VertexNode(Vector3 position)
        {
            Position = position;
            RemainedVisits = 0;
        }

        public int CompareTo(VertexNode other)
        {
            return RemainedVisits.CompareTo(other);
        }
    }

    public struct Portal
    {
        public VertexNode From;
        public VertexNode To;

        public Portal(VertexNode from, VertexNode to)
        {
            From = from;
            To = to;
        }
    }

    public class PortalVertexData: IComparable<PortalVertexData>
    {
        public VertexNode To;
        public float Angle;
        public bool IsVisited;

        public PortalVertexData(VertexNode from, VertexNode to)
        {
            To = to;
            from.RemainedVisits++;
            Vector3 fromDirection = from.Previous.Position - from.Position;
            float angleOffset = Mathf.Atan2(fromDirection.z, fromDirection.x);
            Vector3 toDirection = to.Position - from.Position;
            float angle = Mathf.Atan2(toDirection.z, toDirection.x) - angleOffset;
            if (angle < 0f) { angle += 2f * Mathf.PI; }
            Angle = angle;
            IsVisited = false;
        }

        public int CompareTo(PortalVertexData other)
        {
            return Angle.CompareTo(other.Angle);
        }
    }

    public static List<NavMeshCell> GetNavMesh(List<ANavMGPolygon> polygons)
    {
        //for (int i = 1; i < polygons.Count; ++i)
        //{
        //    Array.Reverse(polygons[i].Vertices);
        //}

        LinkedList<VertexNode> notches = new LinkedList<VertexNode>();
        List<VertexNode> startNodes = new List<VertexNode>(capacity: polygons.Count);

        for (int p = 0; p < polygons.Count; ++p)
        {
            Vector3[] vertices = polygons[p].Vertices;
            int length = vertices.Length;
            int reducedLength = length - 1;
            VertexNode[] vertexNodes = new VertexNode[length];
            for (int i = 0; i < length; ++i)
            {
                vertexNodes[i] = new VertexNode(vertices[i]);
            }
            for (int i = 0; i < length; ++i)
            {
                int previous = (i + reducedLength) % length;
                int next = (i + 1) % length;
                vertexNodes[i].Previous = vertexNodes[previous];
                vertexNodes[i].Next = vertexNodes[next];
                float sign = Sign(vertices[previous], vertices[next], vertices[i]);
                if (sign > 0f) { notches.AddLast(vertexNodes[i]); } 
            }
            startNodes.Add(vertexNodes[0]);
        }

        List<Portal> portals = new List<Portal>();
        Dictionary<VertexNode, List<PortalVertexData>> portalsDictionary = new Dictionary<VertexNode, List<PortalVertexData>>();

        void AddPortal(VertexNode from, VertexNode to)
        {
            Portal newPortal = new Portal(from, to);
            portals.Add(newPortal);

            if (!portalsDictionary.ContainsKey(from))
            { portalsDictionary[from] = new List<PortalVertexData>(); }
            portalsDictionary[from].Add(new PortalVertexData(from, to));

            if (!portalsDictionary.ContainsKey(to))
            { portalsDictionary[to] = new List<PortalVertexData>(); }
            portalsDictionary[to].Add(new PortalVertexData(to, from));
        }

        void TryAddPortal(VertexNode notch, VertexNode portalPoint, int portalIndex)
        {
            bool hasIntersections = false;
            for (int p = 0; p < startNodes.Count; ++p)
            {
                VertexNode workingNode = startNodes[p];
                do
                {
                    if (workingNode != notch && workingNode.Next != notch &&
                        workingNode != portalPoint && workingNode.Next != portalPoint)
                    {
                        if (AreLinesIntersecting(notch.Position, portalPoint.Position,
                            workingNode.Position, workingNode.Next.Position))
                        {
                            hasIntersections = true;
                            break;
                        }
                    }
                    workingNode = workingNode.Next;
                }
                while (workingNode != startNodes[p]);
                if (hasIntersections) { break; }
            }

            for (int i = 0; i < portals.Count; ++i)
            {
                if (i != portalIndex)
                {
                    if (portals[i].From != notch && portals[i].To != notch &&
                        portals[i].From != portalPoint && portals[i].To != portalPoint)
                    {
                        if (AreLinesIntersecting(notch.Position, portalPoint.Position,
                            portals[i].From.Position, portals[i].To.Position))
                        {
                            hasIntersections = true;
                            break;
                        }
                    }
                }
            }

            if (!hasIntersections)
            {
                AddPortal(notch, portalPoint);
            }
        }

        //Debug.Log($"Notches count: {notches.Count}");
        while (notches.Count > 0)
        {
            VertexNode notch = notches.First.Value;

            VertexNode nearestPoint = null;
            Vector3 newPoint = Vector3.zero;
            bool isEdge = false;

            float minDistance = float.PositiveInfinity;
            Vector3 coneOrigin = notch.Position;
            Vector3 coneLeft = notch.Position + (notch.Position - notch.Previous.Position);
            Vector3 coneRight = notch.Position + (notch.Position - notch.Next.Position);
            //Debug.Log($"{coneOrigin} {coneLeft} {coneRight}, {notch.Previous.Position} {notch.Next.Position}");

            for (int p = 0; p < startNodes.Count; ++p)
            {
                VertexNode workingNode = startNodes[p];
                do
                {
                    if (workingNode != notch)
                    {
                        Vector3 difference = notch.Position - workingNode.Position;
                        if (IsPointInCone(workingNode.Position, coneRight, coneOrigin, coneLeft))
                        {
                            float distance = difference.x * difference.x + difference.z * difference.z;
                            if (distance < minDistance)
                            {
                                nearestPoint = workingNode;
                                minDistance = distance;
                                isEdge = false;
                            }
                        }
                        else
                        {
                            if (workingNode.Next != notch)
                            {
                                Vector3 lineDifference = workingNode.Next.Position - workingNode.Position;
                                float lengthSquared = lineDifference.x * lineDifference.x + lineDifference.z * lineDifference.z;

                                float t = (difference.x * lineDifference.x + difference.z * lineDifference.z) / lengthSquared;

                                bool isSpecial = false;
                                if (t >= 0f && t <= 1f)
                                {
                                    Vector3 projection = workingNode.Position + t * lineDifference;
                                    if (IsPointInCone(projection, coneRight, coneOrigin, coneLeft))
                                    {
                                        Vector3 projectionDifference = projection - notch.Position;
                                        float distance = projectionDifference.x * projectionDifference.x + projectionDifference.z * projectionDifference.z;
                                        if (distance < minDistance)
                                        {
                                            nearestPoint = workingNode;
                                            newPoint = projection;
                                            minDistance = distance;
                                            isEdge = true;
                                        }
                                    }
                                    else { isSpecial = true; }
                                }
                                else { isSpecial = true; }
                                if (isSpecial)
                                {
                                    Vector3 intersectionPoint;
                                    if (IsIntersectingCone(coneOrigin, coneLeft, workingNode.Position, workingNode.Next.Position, out intersectionPoint))
                                    {
                                        Vector3 intersectionDifference = intersectionPoint - notch.Position;
                                        float distance = intersectionDifference.x * intersectionDifference.x + intersectionDifference.z * intersectionDifference.z;
                                        if (distance < minDistance)
                                        {
                                            nearestPoint = workingNode;
                                            newPoint = intersectionPoint;
                                            minDistance = distance;
                                            isEdge = true;
                                        }
                                    }
                                    if (IsIntersectingCone(coneOrigin, coneRight, workingNode.Position, workingNode.Next.Position, out intersectionPoint))
                                    {
                                        Vector3 intersectionDifference = intersectionPoint - notch.Position;
                                        float distance = intersectionDifference.x * intersectionDifference.x + intersectionDifference.z * intersectionDifference.z;
                                        if (distance < minDistance)
                                        {
                                            nearestPoint = workingNode;
                                            newPoint = intersectionPoint;
                                            minDistance = distance;
                                            isEdge = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    workingNode = workingNode.Next;
                }
                while (workingNode != startNodes[p]);
            }

            bool isPortal = false;
            int portalIndex = -1;
            for (int i = 0; i < portals.Count; ++i)
            {
                Vector3 notchDifference = notch.Position - portals[i].From.Position;
                Vector3 lineDifference = portals[i].To.Position - portals[i].From.Position;
                float lengthSquared = lineDifference.x * lineDifference.x + lineDifference.z * lineDifference.z;

                float t = (notchDifference.x * lineDifference.x + notchDifference.z * lineDifference.z) / lengthSquared;

                if (t >= 0f && t <= 1f)
                {
                    Vector3 projection = portals[i].From.Position + t * lineDifference;
                    if (IsPointInCone(projection, coneRight, coneOrigin, coneLeft))
                    {
                        Vector3 projectionDifference = projection - notch.Position;
                        float distance = projectionDifference.x * projectionDifference.x + projectionDifference.z * projectionDifference.z;
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            portalIndex = i;
                            isEdge = true;
                            isPortal = true;
                        }
                    }
                }
            }

            // creating portals
            if (isEdge)
            {
                if (isPortal)
                {
                    Portal nearestPortal = portals[portalIndex];
                    bool hasPointInCone = false;
                    minDistance = float.PositiveInfinity;

                    if (IsPointInCone(nearestPortal.From.Position, coneRight, coneOrigin, coneLeft))
                    {
                        Vector3 difference = notch.Position - nearestPortal.From.Position;
                        minDistance = difference.x * difference.x + difference.z * difference.z;
                        nearestPoint = nearestPortal.From;
                        hasPointInCone = true;
                    }

                    if (IsPointInCone(nearestPortal.To.Position, coneRight, coneOrigin, coneLeft))
                    {
                        Vector3 difference = notch.Position - nearestPortal.To.Position;
                        float distance = difference.x * difference.x + difference.z * difference.z;
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestPoint = nearestPortal.To;
                            hasPointInCone = true;
                        }
                    }

                    if (hasPointInCone)
                    {
                        AddPortal(notch, nearestPoint);
                    }
                    else
                    {
                        TryAddPortal(notch, nearestPortal.From, portalIndex);
                        TryAddPortal(notch, nearestPortal.To, portalIndex);
                    }
                }
                else
                {
                    VertexNode newNode = new VertexNode(newPoint);
                    newNode.Previous = nearestPoint;
                    newNode.Next = nearestPoint.Next;

                    nearestPoint.Next.Previous = newNode;
                    nearestPoint.Next = newNode;

                    AddPortal(notch, newNode);
                }
            }
            else
            {
                AddPortal(notch, nearestPoint);
                if (notches.Contains(nearestPoint))
                {
                    if (Sign(notch.Position, nearestPoint.Next.Position, nearestPoint.Position) <= 0f &&
                        Sign(nearestPoint.Previous.Position, notch.Position, nearestPoint.Position) <= 0f)
                    {
                        notches.Remove(nearestPoint);
                    }
                }
            }

            notches.RemoveFirst();
        }
    
        return CreateCells(startNodes, portalsDictionary);
    }

    private static void PrintDictionary(Dictionary<VertexNode, List<PortalVertexData>> portalsDictionary)
    {
        foreach (var kvp in portalsDictionary)
        {
            Debug.LogWarning($"{kvp.Key.Position}, visits = {kvp.Key.RemainedVisits}");
            foreach (var item in kvp.Value)
            {
                Debug.Log($"Item {item.To.Position}, angle = {item.Angle}");
            }
        }
        Debug.Log(" ");
    }

    private static bool IsPointInCone(Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float s1 = Sign(v1, v2, point),
              s2 = Sign(v2, v3, point);
        return s1 > 0 && s2 > 0;
    }

    //public static PolygonsDrawer PolygonsDrawer;

    private static List<NavMeshCell> CreateCells(List<VertexNode> startNodes, 
        Dictionary<VertexNode, List<PortalVertexData>> portalsDictionary)
    {
        List<NavMeshCell> navMeshCells = new List<NavMeshCell>();
        Queue<VertexNode> notVisitedNodes = new Queue<VertexNode>();
        for (int i = 0; i < startNodes.Count; ++i)
        {
            notVisitedNodes.Enqueue(startNodes[i]);
        }

        while (notVisitedNodes.Count > 0)
        {
            VertexNode fromNode = null;
            VertexNode workingNode = notVisitedNodes.Dequeue();
            if (workingNode.RemainedVisits < 0) { continue; }
            List<VertexNode> nodes = new List<VertexNode>();
            do
            {
                nodes.Add(workingNode);
                workingNode.RemainedVisits--;
                if (workingNode.RemainedVisits >= 0)
                {
                    notVisitedNodes.Enqueue(workingNode);
                }
                if (portalsDictionary.ContainsKey(workingNode))
                {
                    List<PortalVertexData> datas = portalsDictionary[workingNode];
                    if (workingNode.RemainedVisits == datas.Count - 1)
                    {
                        datas.Sort();
                    }
                    int nextIndex = -1;
                    bool byPortal = false;
                    if (fromNode != null)
                    {
                        for (int i = datas.Count - 1; i >= 0; --i)
                        {
                            if (datas[i].To == fromNode)
                            {
                                nextIndex = i - 1;
                                byPortal = true;
                                break;
                            }
                        }
                    }
                    fromNode = workingNode;
                    if (byPortal)
                    {
                        if (nextIndex < 0)
                        {
                            workingNode = workingNode.Next;
                        }
                        else
                        {
                            for (int i = nextIndex; i >= 0; --i)
                            {
                                if (!datas[i].IsVisited)
                                {
                                    datas[i].IsVisited = true;
                                    workingNode = datas[i].To;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = datas.Count - 1; i >= 0; --i)
                        {
                            if (!datas[i].IsVisited)
                            {
                                nextIndex = i;
                                break;
                            }
                        }
                        if (nextIndex < 0)
                        {
                            workingNode = workingNode.Next;
                        }
                        else
                        {
                            datas[nextIndex].IsVisited = true;
                            workingNode = datas[nextIndex].To;
                        }
                    }
                }
                else
                {
                    fromNode = workingNode;
                    workingNode = workingNode.Next;
                }
            }
            while (workingNode != nodes[0]);

            Vector3[] positions = new Vector3[nodes.Count];
            for (int i = 0; i < positions.Length; ++i)
            {
                positions[i] = nodes[i].Position;
            }
            //PolygonsDrawer.AddPolygon(positions);
            navMeshCells.Add(new NavMeshCell(positions)); // EarClippingTriangulation.GetTriangles(positions));
        }
        return navMeshCells;
    }

    private static bool AreLinesIntersecting(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
    {
        float p1p2x = point1.x - point2.x;
        float p1p2z = point1.z - point2.z;
        float p3p4x = point3.x - point4.x;
        float p3p4z = point3.z - point4.z;

        float denominator = p1p2x * p3p4z - p1p2z * p3p4x;
        if (Mathf.Approximately(denominator, 0f)) { return false; }
        else
        {
            float part1 = point1.x * point2.z - point1.z * point2.x;
            float part2 = point3.x * point4.z - point3.z * point4.x;
            float x = (part1 * p3p4x - p1p2x * part2) / denominator;
            float z = (part1 * p3p4z - p1p2z * part2) / denominator;

            float dx = x - point1.x;
            float dz = z - point1.z;
            float p1distanceSquared = dx * dx + dz * dz;
            dx = x - point2.x;
            dz = z - point2.z;
            float p2distanceSquared = dx * dx + dz * dz;
            float p1p2distanceSquared = p1p2x * p1p2x + p1p2z * p1p2z;
            if (p1distanceSquared <= p1p2distanceSquared && p2distanceSquared <= p1p2distanceSquared)
            {
                // is on edge
                dx = x - point3.x;
                dz = z - point3.z;
                float p3distanceSquared = dx * dx + dz * dz;
                dx = x - point4.x;
                dz = z - point4.z;
                float p4distanceSquared = dx * dx + dz * dz;
                float p3p4distanceSquared = p3p4x * p3p4x + p3p4z * p3p4z;

                return (p3distanceSquared <= p3p4distanceSquared && p4distanceSquared <= p3p4distanceSquared);
            }
            else
            {
                return false;
            }
        }
    }

    private static bool IsIntersectingCone(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, out Vector3 intersectionPoint)
    {
        float p1p2x = point1.x - point2.x;
        float p1p2z = point1.z - point2.z;
        float p3p4x = point3.x - point4.x;
        float p3p4z = point3.z - point4.z;

        float denominator = p1p2x * p3p4z - p1p2z * p3p4x;
        if (Mathf.Approximately(denominator, 0f)) { intersectionPoint = Vector3.zero; return false; }
        else
        {
            float part1 = point1.x * point2.z - point1.z * point2.x;
            float part2 = point3.x * point4.z - point3.z * point4.x;
            float x = (part1 * p3p4x - p1p2x * part2) / denominator;
            float z = (part1 * p3p4z - p1p2z * part2) / denominator;

            // is in area of interest
            float dx = x - point1.x;
            float dz = z - point1.z;
            float p1distanceSquared = dx * dx + dz * dz;
            dx = x - point2.x;
            dz = z - point2.z;
            float p2distanceSquared = dx * dx + dz * dz;
            float p1p2distanceSquared = p1p2x * p1p2x + p1p2z * p1p2z;
            if ((p1distanceSquared <= p1p2distanceSquared && p2distanceSquared <= p1p2distanceSquared) ||
                p2distanceSquared < p1distanceSquared)
            {
                // is on edge
                dx = x - point3.x;
                dz = z - point3.z;
                float p3distanceSquared = dx * dx + dz * dz;
                dx = x - point4.x;
                dz = z - point4.z;
                float p4distanceSquared = dx * dx + dz * dz;
                float p3p4distanceSquared = p3p4x * p3p4x + p3p4z * p3p4z;

                if (p3distanceSquared <= p3p4distanceSquared && p4distanceSquared <= p3p4distanceSquared)
                {
                    Vector3 difference34 = point4 - point3;
                    float factor = Mathf.Sqrt(p3distanceSquared / p3p4distanceSquared);
                    intersectionPoint = point3 + factor * difference34;
                    return true;
                }
                else
                {
                    intersectionPoint = Vector3.zero;
                    return false;
                }
            }
            else
            {
                intersectionPoint = Vector3.zero;
                return false;
            }
        }
    }

    public static float Sign(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        return (v2.x - v1.x) * (v3.z - v1.z) - (v3.x - v1.x) * (v2.z - v1.z);
    }
}
