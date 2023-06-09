using UnityEngine;

public class IntMatrixDrawer : MonoBehaviour
{
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Color32 _notValidColor;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void Draw(ObstacleLayer obstacleLayer, bool[,] isObstacle)
    {
        Texture2D texture2D = new Texture2D(obstacleLayer.Width, obstacleLayer.Height, TextureFormat.RGBA32, false);
        texture2D.filterMode = FilterMode.Point;

        Color32[] colors = GetColors(isObstacle);

        texture2D.SetPixels32(colors);
        texture2D.Apply();

        SetSprite(obstacleLayer, texture2D);
    }

    private Color32[] GetColors(bool[,] isObstacle)
    {
        int width = isObstacle.GetLength(0);
        int height = isObstacle.GetLength(1);
        Color32[] colors = new Color32[width * height];
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                int colorsIndex = x + y * width;
                float value = isObstacle[x, y] ? 0f : 1f;
                colors[colorsIndex] = _gradient.Evaluate(value);
            }
        }
        return colors;
    }

    private Color32[] GetColors(int[,] values)
    {
        int width = values.GetLength(0);
        int height = values.GetLength(1);
        Color32[] colors = new Color32[width * height];
        int maxValue = 0;
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                if (values[x, y] > maxValue) { maxValue = values[x, y]; }
            }
        }

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                int colorsIndex = x + y * width;
                if (values[x, y] >= 0)
                {
                    colors[colorsIndex] = _gradient.Evaluate((float)values[x, y] / maxValue);
                }
                else
                {
                    colors[colorsIndex] = _notValidColor;
                }
            }
        }
        return colors;
    }

    public void Draw(ObstacleLayer obstacleLayer, int[,] values)
    {
        Texture2D texture2D = new Texture2D(obstacleLayer.Width, obstacleLayer.Height, TextureFormat.RGBA32, false);
        texture2D.filterMode = FilterMode.Point;

        Color32[] colors = GetColors(values);

        texture2D.SetPixels32(colors);
        texture2D.Apply();

        SetSprite(obstacleLayer, texture2D);
    }

    private void SetSprite(ObstacleLayer obstacleLayer, Texture2D texture2D)
    {
        Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, obstacleLayer.Width, obstacleLayer.Height), Vector2.zero, 1);
        _spriteRenderer.sprite = sprite;
        Vector3 rendererPosition = new Vector3(obstacleLayer.Origin.x, 0f, obstacleLayer.Origin.y);
        _spriteRenderer.transform.position = rendererPosition;
        Vector3 rendererScale = new Vector3(obstacleLayer.Step.x, obstacleLayer.Step.y, 1f);
        _spriteRenderer.transform.localScale = rendererScale;
        _spriteRenderer.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
