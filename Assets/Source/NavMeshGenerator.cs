using UnityEngine;

public abstract class NavMeshGenerator : MonoBehaviour
{
    public abstract CellAndPortalGraph Generate(GameObject[] gameObjects);
}
