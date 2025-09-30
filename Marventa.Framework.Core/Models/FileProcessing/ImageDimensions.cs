namespace Marventa.Framework.Core.Models.FileProcessing;

/// <summary>
/// Image dimensions
/// </summary>
public class ImageDimensions
{
    public int Width { get; set; }
    public int Height { get; set; }
    public double AspectRatio => Height > 0 ? (double)Width / Height : 0;
}
