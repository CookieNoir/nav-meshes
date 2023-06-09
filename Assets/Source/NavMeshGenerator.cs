using UnityEngine;

public abstract class NavMeshGenerator : MonoBehaviour
{
    [SerializeField, Min(0.001f)] protected float RasterCellSize;
    [SerializeField] protected ObstacleDetectorSettings ObstacleDetectorSettings;

    public abstract CellAndPortalGraph Generate(GameObject[] gameObjects);
}
