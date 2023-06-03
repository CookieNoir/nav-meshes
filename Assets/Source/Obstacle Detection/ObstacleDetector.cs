using UnityEngine;

public static class ObstacleDetector
{
    public static ObstacleLayer DetectObstacles(GridData gridData, ObstacleDetectorSettings obstacleDetectorSettings)
    {
        float angleRadians = obstacleDetectorSettings.MaxAllowedAngleWithUpAxis * Mathf.Deg2Rad;
        float minAllowedUpCos = Mathf.Cos(angleRadians);
        float maxAllowedDistanceSquared = obstacleDetectorSettings.MaxAllowedDistance * obstacleDetectorSettings.MaxAllowedDistance;

        float stepWidthSquared = gridData.Step.x * gridData.Step.x;
        float stepHeightSquared = gridData.Step.y * gridData.Step.y;

        bool[,] isObstacle = new bool[gridData.Width, gridData.Height];
        Vector3[,] positions = new Vector3[gridData.Width, gridData.Height];

        for (int h = 0; h < gridData.Height; ++h)
        {
            CellData cellData = gridData.GetCellData(0, h);
            positions[0, h] = cellData.GetPosition3D();
            isObstacle[0, h] = cellData.Slope < minAllowedUpCos;
        }

        for (int w = 1; w < gridData.Width; ++w)
        {
            int prevW = w - 1;
            for (int h = 0; h < gridData.Height; ++h)
            {
                CellData cellData = gridData.GetCellData(w, h);
                positions[w, h] = cellData.GetPosition3D();
                isObstacle[w, h] = cellData.Slope < minAllowedUpCos;

                if (!(isObstacle[prevW, h] || isObstacle[w, h]))
                {
                    float depth = positions[w, h].y - positions[prevW, h].y;
                    float distanceSquared = stepWidthSquared + depth * depth;
                    if (distanceSquared > maxAllowedDistanceSquared)
                    {
                        if (depth > 0f) { isObstacle[w, h] = true; }
                        else { isObstacle[prevW, h] = true; }
                    }
                }
            }
        }

        for (int h = 1; h < gridData.Height; ++h)
        {
            int prevH = h - 1;
            for (int w = 0; w < gridData.Width; ++w)
            {
                if (!(isObstacle[w, prevH] || isObstacle[w, h]))
                {
                    float depth = positions[w, h].y - positions[w, prevH].y;
                    float distanceSquared = stepHeightSquared + depth * depth;
                    if (distanceSquared > maxAllowedDistanceSquared)
                    {
                        if (depth > 0f) { isObstacle[w, h] = true; }
                        else { isObstacle[w, prevH] = true; }
                    }
                }
            }
        }

        if (obstacleDetectorSettings.UseRange)
        {
            float lowerBorder = obstacleDetectorSettings.DepthRange.x;
            float upperBorder = obstacleDetectorSettings.DepthRange.y;

            for (int w = 0; w < gridData.Width; ++w)
            {
                for (int h = 0; h < gridData.Height; ++h)
                {
                    if (positions[w, h].y > upperBorder ||
                        positions[w, h].y < lowerBorder)
                    {
                        isObstacle[w, h] = true;
                    }
                }
            }
        }

        return new ObstacleLayer(gridData.Origin, gridData.Step, isObstacle, positions);
    }
}
