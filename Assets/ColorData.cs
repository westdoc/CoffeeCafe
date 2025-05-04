using UnityEngine;

public static class ColorData
{
    public enum ColorOption
    {
        Empty,
        Red,
        Green,
        Blue
    }

    public static Color GetColor(ColorOption option)
    {
        switch (option)
        {
            case ColorOption.Red:
                return Color.red;
            case ColorOption.Green:
                return Color.green;
            case ColorOption.Blue:
                return Color.blue;
            default:
                return Color.white;
        }
    }
}