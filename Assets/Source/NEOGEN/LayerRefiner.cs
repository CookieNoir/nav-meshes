public static class LayerRefiner
{
    public static void RefineLayer(ReducedLayer reducedLayer)
    {
        bool[,] isObstacleTemp = new bool[reducedLayer.Width, reducedLayer.Height];
        int reducedWidth = reducedLayer.Width - 1;
        int reducedHeight = reducedLayer.Height - 1;
        for (int x = 0; x < reducedLayer.Width; ++x)
        {
            for (int y = 0; y < reducedLayer.Height; ++y)
            {
                if (reducedLayer.IsObstacle[x, y])
                {
                    isObstacleTemp[x, y] = true;
                    if (x > 0) isObstacleTemp[x - 1, y] = true;
                    if (x < reducedWidth) isObstacleTemp[x + 1, y] = true;
                    if (y > 0) isObstacleTemp[x, y - 1] = true;
                    if (y < reducedHeight) isObstacleTemp[x, y + 1] = true;
                }
            }
        }

        for (int x = 0; x < reducedLayer.Width; ++x)
        {
            for (int y = 0; y < reducedLayer.Height; ++y)
            {
                if (isObstacleTemp[x, y]) { reducedLayer.IsObstacle[x, y] = true; }
                else
                {
                    int neighborsCount = 0;
                    if (x == 0 || isObstacleTemp[x - 1, y]) { neighborsCount++; }
                    if (x == reducedWidth || isObstacleTemp[x + 1, y]) { neighborsCount++; }
                    if (y == 0 || isObstacleTemp[x, y - 1]) { neighborsCount++; }
                    if (y == reducedHeight || isObstacleTemp[x, y + 1]) { neighborsCount++; }

                    if (neighborsCount >= 3) { reducedLayer.IsObstacle[x, y] = true; }
                }
            }
        }
    }
}
