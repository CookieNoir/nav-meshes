using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PolygonsDrawer : MonoBehaviour
{
    [SerializeField] private List<NavMeshPolygon> _polygons;
    [SerializeField] private Color[] _palette;

    public void SetPolygons(List<RecastPolygon> polygons)
    {
        _polygons = new List<NavMeshPolygon>();
        _polygons.AddRange(polygons);
    }

    public void SetPolygons(List<ANavMGPolygon> polygons)
    {
        _polygons = new List<NavMeshPolygon>();
        _polygons.AddRange(polygons);
    }

    public void AddPolygon(Vector3[] vertices)
    {
        if (_polygons == null) { _polygons = new List<NavMeshPolygon>(); }
        _polygons.Add(new NavMeshPolygon(vertices.ToList()));
    }

    private void OnDrawGizmos()
    {
        if (_polygons == null) { return; }
        for (int p = 0; p < _polygons.Count; ++p)
        {
            if (_palette == null || _palette.Length == 0) { Gizmos.color = Color.green; }
            else { Gizmos.color = _palette[p % _palette.Length]; }
            Vector3[] vertices = _polygons[p].Vertices;
            for (int i = 0; i < vertices.Length; ++i)
            {
                Gizmos.DrawLine(vertices[i], vertices[(i + 1) % vertices.Length]);
            }
        }
    }
}
