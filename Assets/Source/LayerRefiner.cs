public static class LayerRefiner
{
    public static void RefineLayer(ObstacleLayer layer)
    {
        bool[,] isObstacleTemp = new bool[layer.Width, layer.Height];
        int reducedWidth = layer.Width - 1;
        int reducedHeight = layer.Height - 1;
        for (int x = 0; x < layer.Width; ++x)
        {
            for (int y = 0; y < layer.Height; ++y)
            {
                if (layer.IsObstacle[x, y])
                {
                    isObstacleTemp[x, y] = true;
                    if (x > 0) isObstacleTemp[x - 1, y] = true;
                    if (x < reducedWidth) isObstacleTemp[x + 1, y] = true;
                    if (y > 0) isObstacleTemp[x, y - 1] = true;
                    if (y < reducedHeight) isObstacleTemp[x, y + 1] = true;
                }
            }
        }

        for (int x = 0; x < layer.Width; ++x)
        {
            for (int y = 0; y < layer.Height; ++y)
            {
                if (isObstacleTemp[x, y]) { layer.IsObstacle[x, y] = true; }
                else
                {
                    int neighborsCount = 0;
                    if (x == 0 || isObstacleTemp[x - 1, y]) { neighborsCount++; }
                    if (x == reducedWidth || isObstacleTemp[x + 1, y]) { neighborsCount++; }
                    if (y == 0 || isObstacleTemp[x, y - 1]) { neighborsCount++; }
                    if (y == reducedHeight || isObstacleTemp[x, y + 1]) { neighborsCount++; }

                    if (neighborsCount >= 3) { layer.IsObstacle[x, y] = true; }
                }
            }
        }

        layer.SetBounds();
    }
}
