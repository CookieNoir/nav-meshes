using UnityEngine;

public class NavMeshGeneratorStarter : MonoBehaviour
{
    [SerializeField] private NavMeshGenerator _navMeshGenerator;
    [SerializeField] private GameObject[] _gameObjects;

    private void Start()
    {
        _navMeshGenerator.Generate(_gameObjects);
    }
}
