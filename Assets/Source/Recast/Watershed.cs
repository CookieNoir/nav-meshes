using System;
using System.Collections.Generic;
using UnityEngine;

public static class Watershed
{
    public class PixelData : IComparable<PixelData>
    {
        public int Width;
        public int Height;
        public int Value;
        public PixelData[] Neighbors;

        public PixelData(int width, int height, int value)
        {
            Width = width;
            Height = height;
            Value = value;
            Neighbors = new PixelData[4];
        }

        public int CompareTo(PixelData other)
        {
            return Value.CompareTo(other.Value);
        }
    }

    private static int _mask = -2;
    private static int _init = -1;
    private static int _wshed = 0;
    private static int _inQueue = -3;

    private static readonly int[] _neighborOffsetX = new int[] { 0, 1, 0, -1 };
    private static readonly int[] _neighborOffsetY = new int[] { 1, 0, -1, 0 };

    public static int[,] GetPartition(int[,] imageInput)
    {
        int width = imageInput.GetLength(0);
        int height = imageInput.GetLength(1);

        bool IsPositionValid(int x, int y)
        {
            return (x >= 0 && x < width && y >= 0 && y < height);
        }

        int totalPixelsCount = width * height;
        int[,] imageOutput = new int[width, height];
        List<PixelData> sortedPixels = new List<PixelData>(capacity: totalPixelsCount);
        PixelData[,] pixelDatas = new PixelData[width, height];

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                if (imageInput[x, y] == 0)
                {
                    imageOutput[x, y] = _wshed;
                }
                else
                {
                    imageOutput[x, y] = _init;
                }
                PixelData pixelData = new PixelData(x, y, -imageInput[x, y]);
                pixelDatas[x, y] = pixelData;
                sortedPixels.Add(pixelData);
            }
        }

        // Set neighbors
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                for (int n = 0; n < 4; ++n)
                {
                    int neighborWidth = pixelDatas[x, y].Width + _neighborOffsetX[n];
                    int neighborHeight = pixelDatas[x, y].Height + _neighborOffsetY[n];
                    if (IsPositionValid(neighborWidth, neighborHeight))
                    {
                        pixelDatas[x, y].Neighbors[n] = pixelDatas[neighborWidth, neighborHeight];
                    }
                }
            }
        }

        int currentLabel = 0;

        sortedPixels.Sort();
        int h_min = sortedPixels[0].Value;
        int h_max = sortedPixels[sortedPixels.Count - 1].Value;
        int levelsCount = h_max - h_min + 1;

        int currentLevel = 0;
        int[] levelStartIndices = new int[levelsCount + 1];
        for (int i = 0; i < sortedPixels.Count; ++i)
        {
            if (sortedPixels[i].Value - h_min > currentLevel)
            {
                while (sortedPixels[i].Value - h_min > currentLevel)
                {
                    currentLevel++;
                }
                levelStartIndices[currentLevel] = i;
            }
        }
        levelStartIndices[levelsCount] = totalPixelsCount;

        Queue<PixelData> queue = new Queue<PixelData>();

        for (int h = h_min; h < h_max; ++h)
        {
            int levelIndex = h - h_min;
            for (int p = levelStartIndices[levelIndex]; p < levelStartIndices[levelIndex + 1]; ++p)
            {
                PixelData pixel = sortedPixels[p];
                imageOutput[pixel.Width, pixel.Height] = _mask;

                for (int n = 0; n < 4; ++n)
                {
                    PixelData neighbor = pixel.Neighbors[n];
                    if (neighbor != null && imageOutput[neighbor.Width, neighbor.Height] >= _wshed)
                    {
                        imageOutput[pixel.Width, pixel.Height] = _inQueue;
                        queue.Enqueue(pixel);
                        break;
                    }
                }
            }

            // Extend basins
            while (queue.TryDequeue(out PixelData pixel))
            {
                bool flag = false;
                for (int n = 0; n < 4; ++n)
                {
                    PixelData neighbor = pixel.Neighbors[n];
                    if (neighbor != null)
                    {
                        int imo_pixel = imageOutput[pixel.Width, pixel.Height];
                        int imo_neighbor = imageOutput[neighbor.Width, neighbor.Height];

                        if (imo_neighbor > 0)
                        {
                            if (imo_pixel == _inQueue || (imo_pixel == _wshed && flag))
                            {
                                imageOutput[pixel.Width, pixel.Height] = imo_neighbor;
                            }
                            else if (imo_pixel > 0 && imo_pixel != imo_neighbor)
                            {
                                imageOutput[pixel.Width, pixel.Height] = _wshed;
                                flag = false;
                            }
                        }
                        else if (imo_neighbor == _wshed)
                        {
                            if (imo_pixel == _inQueue)
                            {
                                imageOutput[pixel.Width, pixel.Height] = _wshed;
                                flag = true;
                            }
                        }
                        else if (imo_neighbor == _mask)
                        {
                            imageOutput[neighbor.Width, neighbor.Height] = _inQueue;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            // Check if new minima have been discovered
            for (int p = levelStartIndices[levelIndex]; p < levelStartIndices[levelIndex + 1]; ++p)
            {
                PixelData pixel = sortedPixels[p];
                if (imageOutput[pixel.Width, pixel.Height] == _mask)
                {
                    currentLabel++;
                    queue.Enqueue(pixel);
                    imageOutput[pixel.Width, pixel.Height] = currentLabel;
                    while (queue.TryDequeue(out PixelData pixelNeighbor))
                    {
                        for (int n = 0; n < 4; ++n)
                        {
                            PixelData secondNeighbor = pixelNeighbor.Neighbors[n];
                            if (secondNeighbor != null)
                            {
                                if (imageOutput[secondNeighbor.Width, secondNeighbor.Height] == _mask)
                                {
                                    queue.Enqueue(secondNeighbor);
                                    imageOutput[secondNeighbor.Width, secondNeighbor.Height] = currentLabel;
                                }
                            }
                        }
                    }
                }
            }
        }

        return imageOutput;
    }

    public static void Expand(int[,] watershedPartition)
    {
        int width = watershedPartition.GetLength(0);
        int height = watershedPartition.GetLength(1);

        bool IsValid(int x, int y)
        {
            return (x >= 0 && x < width && y >= 0 && y < height);
        }

        int HasSingleNeighbor(int x, int y)
        {
            int result = -1;
            for (int n = 0; n < 4; ++n)
            {
                int w = x + _neighborOffsetX[n];
                int h = y + _neighborOffsetY[n];
                if (IsValid(w, h) && watershedPartition[w, h] > 0)
                {
                    if (result == -1)
                    {
                        result = watershedPartition[w, h];
                    }
                    else
                    {
                        if (watershedPartition[w, h] != result)
                        {
                            return -1;
                        }
                    }
                }
            }
            return result;
        }

        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        bool Visit(int x, int y)
        {
            if (watershedPartition[x, y] == _wshed)
            {
                int neighbor = HasSingleNeighbor(x, y);
                if (neighbor > 0)
                {
                    watershedPartition[x, y] = neighbor;
                    bool result = false;
                    for (int n = 0; n < 4; ++n)
                    {
                        int w = x + _neighborOffsetX[n];
                        int h = y + _neighborOffsetY[n];
                        if (IsValid(w, h))
                        {
                            stack.Push(new Vector2Int(w, h));
                            result = true;
                        }
                    }
                    return result;
                }
                else return false;
            }
            else return false;
        }

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                if (Visit(x, y))
                {
                    while (stack.TryPop(out Vector2Int item))
                    {
                        Visit(item.x, item.y);
                    }
                }
            }
        }
    }
}
