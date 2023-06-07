using System.Collections.Generic;
using UnityEngine;

public static class ANavMGPolygonExtractor
{
    private static readonly Vector2Int[] _directionsLeft = new Vector2Int[4]
    {
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
    };

    private static readonly Vector2Int[,] _directions = new Vector2Int[4, 3]
    {
        { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), },
        { new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(1, -1),},
        { new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1),},
        { new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1), },
    };

    public static List<Polygon> GetPolygons(ObstacleLayer reducedLayer)
    {
        List<Polygon> polygons = new List<Polygon>();
        bool[,] isObstacle = reducedLayer.IsObstacle;
        Vector3[,] positions = reducedLayer.Positions;
        bool[,] isVisited = new bool[reducedLayer.Width, reducedLayer.Height];

        for (int x = 0; x < reducedLayer.Width; ++x)
        {
            for (int y = 0; y < reducedLayer.Height; ++y)
            {
                if (!isVisited[x, y])
                {
                    isVisited[x, y] = true;
                    Vector2Int currentPosition = new Vector2Int(x, y);
                    if (isObstacle[x, y])
                    {
                        int direction = 0;
                        while (direction < 4)
                        {
                            if (!IsBlack(isObstacle, currentPosition + _directionsLeft[direction])) { break; }
                            direction++;
                        }
                        if (direction < 4) { polygons.Add(GetPolygon(isObstacle, positions, isVisited, currentPosition, direction)); }
                    }
                }
            }
        }
        return polygons;
    }

    private static bool IsBlack(bool[,] isObstacle, Vector2Int position)
    {
        if (position.x < 0 || position.x >= isObstacle.GetLength(0) ||
            position.y < 0 || position.y >= isObstacle.GetLength(1)) { return false; }
        else { return isObstacle[position.x, position.y]; }
    }

    private static Polygon GetPolygon(bool[,] isObstacle, Vector3[,] positions, bool[,] isVisited, Vector2Int startPosition, int startDirection)
    {
        List<Vector3> polygonVertices = new List<Vector3>();
        Vector2Int currentPosition;
        int rotations;
        int currentDirection = startDirection;

        void Move(Vector2Int newPosition)
        {
            currentPosition = newPosition;
            polygonVertices.Add(positions[currentPosition.x, currentPosition.y]);
            isVisited[currentPosition.x, currentPosition.y] = true;
            rotations = 0;
        }

        Move(startPosition);
        while (true)
        {
            Vector2Int leftPosition = currentPosition + _directions[currentDirection, 0];
            Vector2Int forwardPosition = currentPosition + _directions[currentDirection, 1];
            Vector2Int rightPosition = currentPosition + _directions[currentDirection, 2];
            if (IsBlack(isObstacle, leftPosition))
            {
                if (leftPosition == startPosition) { break; }
                else
                {
                    Move(leftPosition);
                    currentDirection = (currentDirection + 3) % 4;
                }
            }
            else if (IsBlack(isObstacle, forwardPosition))
            {
                if (forwardPosition == startPosition) { break; }
                else { Move(forwardPosition); }
            }
            else if (IsBlack(isObstacle, rightPosition))
            {
                if (rightPosition == startPosition) { break; }
                else { Move(rightPosition); }
            }
            else if (rotations == 3) { break; }
            else
            {
                currentDirection = (currentDirection + 1) % 4;
                rotations++;
            }
        }
        return new Polygon(polygonVertices);
    }
}
