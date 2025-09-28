namespace Marventa.Framework.Core.Models.ML;

/// <summary>
/// Face detection result
/// </summary>
public class FaceDetectionResult
{
    /// <summary>
    /// Detected faces
    /// </summary>
    public DetectedFace[] Faces { get; set; } = Array.Empty<DetectedFace>();

    /// <summary>
    /// Total face count
    /// </summary>
    public int FaceCount => Faces.Length;

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Model version used for detection
    /// </summary>
    public string ModelVersion { get; set; } = string.Empty;

    /// <summary>
    /// Image dimensions
    /// </summary>
    public ImageDimensions ImageDimensions { get; set; } = new();

    /// <summary>
    /// Whether detection was successful
    /// </summary>
    public bool Success { get; set; }
}

/// <summary>
/// Detected face with optional analysis
/// </summary>
public class DetectedFace
{
    public BoundingBox BoundingBox { get; set; } = new();
    public double Confidence { get; set; }
    public FacialLandmarks? Landmarks { get; set; }
    public Dictionary<string, double>? Emotions { get; set; }
    public FaceDemographics? Demographics { get; set; }
}

/// <summary>
/// Face demographics information
/// </summary>
public class FaceDemographics
{
    public AgeRange AgeRange { get; set; } = new();
    public string Gender { get; set; } = string.Empty;
    public double GenderConfidence { get; set; }
}

/// <summary>
/// Facial landmarks for detailed face analysis
/// </summary>
public class FacialLandmarks
{
    public Point LeftEye { get; set; } = new();
    public Point RightEye { get; set; } = new();
    public Point Nose { get; set; } = new();
    public Point MouthLeft { get; set; } = new();
    public Point MouthRight { get; set; } = new();
    public Point[] AllLandmarks { get; set; } = Array.Empty<Point>();
}

/// <summary>
/// Emotion analysis for faces
/// </summary>
public class EmotionAnalysis
{
    public Dictionary<EmotionType, double> Emotions { get; set; } = new();
    public EmotionType DominantEmotion { get; set; }
    public double Confidence { get; set; }
}

/// <summary>
/// Demographic analysis for faces
/// </summary>
public class DemographicAnalysis
{
    public AgeRange EstimatedAge { get; set; } = new();
    public Gender EstimatedGender { get; set; }
    public double GenderConfidence { get; set; }
}

/// <summary>
/// Age range estimation
/// </summary>
public class AgeRange
{
    public int Min { get; set; }
    public int Max { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public int EstimatedAge { get; set; }
    public double Confidence { get; set; }
}