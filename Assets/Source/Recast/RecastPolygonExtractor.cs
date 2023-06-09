using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RecastPolygonExtractor
{
    private static readonly Vector2Int[] _directionsLeft = new Vector2Int[8]
    {
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),

        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
        new Vector2Int(1, -1),
    };

    private static readonly Vector2Int[,] _directions = new Vector2Int[4, 3]
    {
        { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), },
        { new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(1, -1),},
        { new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1),},
        { new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1), },
    };

    public static List<RecastPolygon> GetPolygons(ObstacleLayer obstacleLayer, int[,] watershedPartition, out bool[,] isObstacle)
    {
        //Watershed.Expand(watershedPartition);

        isObstacle = new bool[obstacleLayer.Width, obstacleLayer.Height];
        int maxValue = 0;
        for (int x = 0; x < obstacleLayer.Width; ++x)
        {
            for (int y = 0; y < obstacleLayer.Height; ++y)
            {
                if (obstacleLayer.IsObstacle[x, y])
                {
                    isObstacle[x, y] = true;
                }
                else
                {
                    isObstacle[x, y] = watershedPartition[x, y] <= 0;
                }
                if (watershedPartition[x, y] > maxValue)
                {
                    maxValue = watershedPartition[x, y];
                }
            }
        }

        List<RecastPolygon> polygons = new List<RecastPolygon>(capacity: maxValue);
        Vector3[,] positions = obstacleLayer.Positions;
        bool[] isCreated = new bool[maxValue];

        for (int x = 0; x < obstacleLayer.Width; ++x)
        {
            for (int y = 0; y < obstacleLayer.Height; ++y)
            {
                Vector2Int currentPosition = new Vector2Int(x, y);
                if (isObstacle[x, y])
                {
                    int direction = 0;
                    while (direction < 4)
                    {
                        if (!IsBlack(isObstacle, currentPosition + _directionsLeft[direction])) { break; }
                        direction++;
                    }
                    if (direction < 4)
                    {
                        int index = watershedPartition[currentPosition.x + _directionsLeft[direction].x,
                                                       currentPosition.y + _directionsLeft[direction].y] - 1;
                        if (!isCreated[index])
                        {
                            polygons.Add(GetPolygon(index, watershedPartition, isObstacle, positions, currentPosition, direction));
                            isCreated[index] = true;
                        }               
                    }
                }
            }
        }

        return polygons;
    }

    private static bool IsBlack(bool[,] isObstacle, Vector2Int position)
    {
        if (position.x < 0 || position.x >= isObstacle.GetLength(0) ||
            position.y < 0 || position.y >= isObstacle.GetLength(1)) { return true; }
        else { return isObstacle[position.x, position.y]; }
    }

    private static RecastPolygon GetPolygon(int index, int[,] watershed, bool[,] isObstacle, Vector3[,] positions, Vector2Int startPosition, int startDirection)
    {
        List<Vector3> polygonVertices = new List<Vector3>();
        List<HashSet<int>> polygonNeighbors = new List<HashSet<int>>();
        Vector2Int currentPosition;
        int rotations;
        int currentDirection = startDirection;

        HashSet<int> GetNeighborIndices(Vector2Int position)
        {
            HashSet<int> neighbors = new HashSet<int>();
            for (int n = 0; n < 4; ++n)
            {
                Vector2Int neighborPosition = position + _directionsLeft[n];
                if (!IsBlack(isObstacle, neighborPosition))
                {
                    int neighborIndex = watershed[neighborPosition.x, neighborPosition.y] - 1;
                    neighbors.Add(neighborIndex);
                }
            }
            return neighbors;
        }

        bool HasRequiredNeighbor(Vector2Int position)
        {
            for (int n = 0; n < 4; ++n)
            {
                Vector2Int neighborPosition = position + _directionsLeft[n];
                if (!IsBlack(isObstacle, neighborPosition))
                {
                    int neighborIndex = watershed[neighborPosition.x, neighborPosition.y] - 1;
                    if (neighborIndex == index) { return true; }
                }
            }
            return false;
        }

        void Move(Vector2Int newPosition)
        {
            currentPosition = newPosition;
            polygonVertices.Add(positions[currentPosition.x, currentPosition.y]);
            polygonNeighbors.Add(GetNeighborIndices(currentPosition));
            rotations = 0;
        }

        Move(startPosition);
        while (true)
        {
            Vector2Int leftPosition = currentPosition + _directions[currentDirection, 0];
            Vector2Int forwardPosition = currentPosition + _directions[currentDirection, 1];
            Vector2Int rightPosition = currentPosition + _directions[currentDirection, 2];
            if (IsBlack(isObstacle, leftPosition) && HasRequiredNeighbor(leftPosition))
            {
                if (leftPosition == startPosition) { break; }
                else
                {
                    Move(leftPosition);
                    currentDirection = (currentDirection + 3) % 4;
                }
            }
            else if (IsBlack(isObstacle, forwardPosition) && HasRequiredNeighbor(forwardPosition))
            {
                if (forwardPosition == startPosition) { break; }
                else { Move(forwardPosition); }
            }
            else if (IsBlack(isObstacle, rightPosition) && HasRequiredNeighbor(rightPosition))
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
        return new RecastPolygon(index, polygonVertices, polygonNeighbors);
    }
}
