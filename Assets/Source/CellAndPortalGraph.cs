using System.Collections.Generic;
using UnityEngine;

public class CellAndPortalGraph
{
    private List<NavMeshCell> _cells;

    public CellAndPortalGraph(List<NavMeshCell> cells)
    {
        _cells = cells;
    }

    public bool TryGetCell(Vector2 position, out NavMeshCell navMeshCell)
    {
        navMeshCell = null;
        if (_cells == null) { return false; }
        
        return true;
    }

    public NavMeshCell GetNearestCell(Vector2 position)
    {
        if (_cells == null) { return null; }
        
        return null;
    }
}
