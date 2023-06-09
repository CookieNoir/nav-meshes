using System.Collections.Generic;
using UnityEngine;

public class CellAndPortalGraph
{
    public List<NavMeshCell> Cells { get; private set; }

    public CellAndPortalGraph(List<NavMeshCell> cells)
    {
        Cells = cells;
    }

    public bool TryGetCell(Vector2 position, out NavMeshCell navMeshCell)
    {
        navMeshCell = null;
        if (Cells == null) { return false; }
        
        return true;
    }

    public NavMeshCell GetNearestCell(Vector2 position)
    {
        if (Cells == null) { return null; }
        
        return null;
    }
}
