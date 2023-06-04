using UnityEngine;

public class ReducedLayerBounds
{
    public int MinWidth { get; private set; }
    public int MinHeight { get; private set; }
    public int MaxWidth { get; private set; }
    public int MaxHeight { get; private set; }

    public ReducedLayerBounds(int width, int height)
    {
        MinWidth = width;
        MaxWidth = width;
        MinHeight = height;
        MaxHeight = height;
    }

    public void UpdateBounds(int width, int height)
    {
        if (width < MinWidth) { MinWidth = width; }
        if (width > MaxWidth) { MaxWidth = width; }
        if (height < MinHeight) { MinHeight = height; }
        if (height > MaxHeight) { MaxHeight = height; }
    }
}
