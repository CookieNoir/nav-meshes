using UnityEngine;

public static class CubeCoordinates
{
    public static Vector3Int[] neighbors = new Vector3Int[] {
        new Vector3Int(1, -1, 0),
        new Vector3Int(1, 0, -1),
        new Vector3Int(0, 1, -1),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(-1, 0, 1),
        new Vector3Int(0, -1, 1)
    };

    public static Vector3 CubeFlatTopToCartesianXZ(Vector3Int cubeCoordinates, Vector2 hexSize)
    {
        return new Vector3(
            (1.5f * cubeCoordinates.x) * hexSize.x,
            0f,
            (-0.86602540378f * cubeCoordinates.x - 1.73205080757f * cubeCoordinates.z) * hexSize.y
            );
    }

    public static Vector3 CubePointyTopToCartesianXZ(Vector3Int cubeCoordinates, Vector2 hexSize)
    {
        return new Vector3(
            (1.73205080757f * cubeCoordinates.x + 0.86602540378f * cubeCoordinates.z) * hexSize.x,
            0f,
            (-1.5f * cubeCoordinates.z) * hexSize.y
            );
    }

    public static Vector3Int CartesianXZToCubeFlatTop(Vector3 coordinates, Vector2 hexSize)
    {
        float q = (0.66666666666f * coordinates.x) / hexSize.x;
        float r = (-0.33333333333f * coordinates.x - 0.57735026919f * coordinates.z) / hexSize.y;

        return CubeRound(new Vector3(q, -q - r, r));
    }

    public static Vector3Int CartesianXZToCubePointyTop(Vector3 coordinates, Vector2 hexSize)
    {
        float q = (0.57735026919f * coordinates.x + 0.33333333333f * coordinates.z) / hexSize.x;
        float r = (-0.66666666666f * coordinates.z) / hexSize.y;

        return CubeRound(new Vector3(q, -q - r, r));
    }

    private static Vector3Int CubeRound(Vector3 cubeCoordinates)
    {
        int rx = Mathf.RoundToInt(cubeCoordinates.x);
        int ry = Mathf.RoundToInt(cubeCoordinates.y);
        int rz = Mathf.RoundToInt(cubeCoordinates.z);

        float xDif = Mathf.Abs(rx - cubeCoordinates.x);
        float yDif = Mathf.Abs(ry - cubeCoordinates.y);
        float zDif = Mathf.Abs(rz - cubeCoordinates.z);

        if (xDif > yDif && xDif > zDif)
            rx = -ry - rz;
        else if (yDif > zDif)
            ry = -rx - rz;
        else
            rz = -rx - ry;
        return new Vector3Int(rx, ry, rz);
    }

    public static Vector3Int GetNeighbor(Vector3Int cubeCoordinates, int direction, int multiplier = 1)
    {
        return cubeCoordinates + multiplier * neighbors[direction];
    }
}