using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    private HexagonContent[,] hexagons;
    public UniversalHexagon singleHexagonGrid;
    public RelaxationProperties relaxationProperties;
    public GameObject hexagonPrefab;
    public Transform prefabParent;
    [Space(15)]
    public int seed;
    [Range(3, 15)] public int neighborRadius = 3;
    public bool spawnObstacles;

    private int neighborRadiusBidirectional;

    public void BuildMap()
    {
        DestroyAllHexagons();
        Random.InitState(seed);
        CreateStartGrid();
        RelaxAtStart();
    }

    private void CreateStartGrid()
    {
        singleHexagonGrid.FillStaticFields();
        neighborRadiusBidirectional = neighborRadius + neighborRadius + 1;
        hexagons = new HexagonContent[neighborRadiusBidirectional, neighborRadiusBidirectional];
        Vector3Int cubeCoordinate;

        hexagons[neighborRadius, neighborRadius] = Instantiate(
                hexagonPrefab,
                Vector3.zero,
                Quaternion.identity).GetComponent<HexagonContent>();
        hexagons[neighborRadius, neighborRadius].transform.parent = prefabParent;
        for (int i = 1; i <= neighborRadius; ++i)
        {
            cubeCoordinate = i * CubeCoordinates.neighbors[4];
            for (int j = 0; j < 6; ++j)
            {
                for (int k = 0; k < i; ++k)
                {
                    cubeCoordinate = CubeCoordinates.GetNeighbor(cubeCoordinate, j);
                    GameObject newGo = Instantiate(
                            hexagonPrefab,
                            CubeCoordinates.CubePointyTopToCartesianXZ(cubeCoordinate, UniversalHexagon.hexagonSize),
                            Quaternion.identity);
                    newGo.name = cubeCoordinate.x.ToString() + " " + cubeCoordinate.z.ToString();
                    hexagons[neighborRadius + cubeCoordinate.x, neighborRadius + cubeCoordinate.z] =
                        newGo.GetComponent<HexagonContent>();
                    hexagons[neighborRadius + cubeCoordinate.x, neighborRadius + cubeCoordinate.z].transform.parent = prefabParent;
                }
            }
        }
    }

    private void TryRelaxTriplet(int direction,
        HexagonContent central,
        HexagonContent directionNeighbor,
        HexagonContent directionMinusOneNeighbor)
    {
        if (central == null || directionNeighbor == null || directionMinusOneNeighbor == null) { return; }
        TripletRelaxator.RelaxTriplet(direction, central.relaxationData, directionNeighbor.relaxationData, directionMinusOneNeighbor.relaxationData);
    }

    private void RelaxAtStart()
    {
        relaxationProperties.ApplyProperties();
        if (RelaxationProperties.iterationsCount > 0)
        {
            Vector3Int cubeCoordinate;
            for (int i = 1; i < neighborRadius; ++i)
            {
                cubeCoordinate = i * CubeCoordinates.neighbors[4];
                for (int j = 0; j < 6; ++j)
                {
                    for (int k = 0; k < i; ++k)
                    {
                        cubeCoordinate = CubeCoordinates.GetNeighbor(cubeCoordinate, j);
                        for (int d = 0; d < 6; ++d)
                        {
                            Vector3Int direction = CubeCoordinates.GetNeighbor(cubeCoordinate, d);
                            Vector3Int directionMinusOne = CubeCoordinates.GetNeighbor(cubeCoordinate, (d + 5) % 6);
                            TryRelaxTriplet(d,
                                hexagons[neighborRadius + cubeCoordinate.x, neighborRadius + cubeCoordinate.z],
                                hexagons[neighborRadius + direction.x, neighborRadius + direction.z],
                                hexagons[neighborRadius + directionMinusOne.x, neighborRadius + directionMinusOne.z]);
                        }
                    }
                }
            }
        }
    }

    private void DestroyAllHexagons()
    {
        for (int i = 0; i < neighborRadiusBidirectional; ++i)
        {
            for (int j = 0; j < neighborRadiusBidirectional; ++j)
            {
                if (hexagons[i, j]) Destroy(hexagons[i, j].gameObject);
            }
        }
        hexagons = null;
    }
}