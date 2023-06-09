using System;

public static class DistanceTransform
{
    private static int _infinity;

    public static int[,] GetDistancesManhattan(ObstacleLayer obstacleLayer)
    {
        bool[,] b = obstacleLayer.IsObstacle;
        int m = obstacleLayer.Width;
        int n = obstacleLayer.Height;
        _infinity = m + n;
        int[,] g = new int[m, n];
        // First phase
        for (int x = 0; x < m; ++x)
        {
            if (b[x, 0])
            {
                g[x, 0] = 0;
            }
            else
            {
                g[x, 0] = _infinity;
            }
            // Scan 1
            for (int y = 1; y < n; ++y)
            {
                if (b[x, y])
                {
                    g[x, y] = 0;
                }
                else
                {
                    g[x, y] = 1 + g[x, y - 1];
                }
            }
            // Scan 2
            for (int y = n - 2; y >= 0; --y)
            {
                if (g[x, y + 1] < g[x, y])
                {
                    g[x, y] = 1 + g[x, y + 1];
                }
            }
        }
        // Second phase
        int[,] dt = new int[m, n];
        int[] s = new int[m];
        int[] t = new int[m];
        for (int y = 0; y < n; ++y)
        {
            int q = 0; s[0] = 0; t[0] = 0;
            // Scan 3
            for (int u = 1; u < m; ++u)
            {
                while (q >= 0 && MDT(t[q], s[q], g[s[q], y]) > MDT(t[q], u, g[u, y]))
                {
                    q--;
                }
                if (q < 0)
                {
                    q = 0; s[0] = u;
                }
                else
                {
                    int w = 1 + MDT_Sep(s[q], u, g[s[q], y], g[u, y]);
                    if (w < m)
                    {
                        q++; s[q] = u; t[q] = w;
                    }
                }
            }
            // Scan 4
            for (int u = m - 1; u >= 0; --u)
            {
                dt[u, y] = MDT(u, s[q], g[s[q], y]);
                if (u == t[q]) { q--; }
            }
        }
        return dt;
    }

    private static int MDT(int x, int i, int g_i)
    {
        return Math.Abs(x - i) + g_i;
    }

    private static int MDT_Sep(int i, int u, int g_i, int g_u)
    {
        if (g_u >= (g_i + u - i))
        {
            return _infinity;
        }
        if (g_i > (g_u + u - i))
        {
            return -_infinity;
        }
        return (g_u - g_i + u + i) / 2;
    }
}
