using System.Collections.Generic;
using UnityEngine;

public class MultipleIntMatrixDrawer : MonoBehaviour
{
    [SerializeField] private GameObject _drawerPrefab;

    public void Draw(List<ObstacleLayer> obstacleLayers)
    {
        Clear();
        foreach (ObstacleLayer obstacleLayer in obstacleLayers)
        {
            GameObject instance = Instantiate(_drawerPrefab, transform);
            if (instance.TryGetComponent(out IntMatrixDrawer intMatrixDrawer))
            {
                intMatrixDrawer.Draw(obstacleLayer, obstacleLayer.IsObstacle);
            }
        }
    }

    private void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
