namespace Marventa.Framework.Core.Models.ML;

/// <summary>
/// Bounding box coordinates
/// </summary>
public class BoundingBox
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

/// <summary>
/// Point coordinates
/// </summary>
public class Point
{
    public double X { get; set; }
    public double Y { get; set; }
}

/// <summary>
/// Image dimensions with aspect ratio calculation
/// </summary>
public class ImageDimensions
{
    public int Width { get; set; }
    public int Height { get; set; }
    public double AspectRatio => Height > 0 ? (double)Width / Height : 0;
}

/// <summary>
/// RGB color representation
/// </summary>
public class RGBColor
{
    public int Red { get; set; }
    public int Green { get; set; }
    public int Blue { get; set; }
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }
}

/// <summary>
/// HSL color representation
/// </summary>
public class HSLColor
{
    public double Hue { get; set; }
    public double Saturation { get; set; }
    public double Lightness { get; set; }
}

/// <summary>
/// Color palette information
/// </summary>
public class ColorPalette
{
    public ColorInfo[] Colors { get; set; } = Array.Empty<ColorInfo>();
    public string PaletteType { get; set; } = string.Empty;
    public double Harmony { get; set; }
}

/// <summary>
/// Color distribution statistics
/// </summary>
public class ColorStatistics
{
    public double Brightness { get; set; }
    public double Contrast { get; set; }
    public double Saturation { get; set; }
    public double Colorfulness { get; set; }
    public string DominantColorFamily { get; set; } = string.Empty;
}