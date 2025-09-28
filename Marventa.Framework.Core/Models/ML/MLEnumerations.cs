namespace Marventa.Framework.Core.Models.ML;

/// <summary>
/// Content moderation categories
/// </summary>
public enum ModerationCategory
{
    Adult,
    Violence,
    Hate,
    SelfHarm,
    Harassment,
    Drugs,
    Weapons,
    Gore,
    Nudity,
    Profanity,
    Spam
}

/// <summary>
/// Moderation severity levels
/// </summary>
public enum ModerationSeverity
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Quality issue types
/// </summary>
public enum QualityIssueType
{
    Blur,
    Noise,
    Overexposure,
    Underexposure,
    PoorContrast,
    LowResolution,
    Artifacts,
    ColorCasting,
    PoorComposition
}

/// <summary>
/// Sentiment types
/// </summary>
public enum SentimentType
{
    Positive,
    Negative,
    Neutral,
    Mixed
}

/// <summary>
/// Spam check types
/// </summary>
public enum SpamCheckType
{
    General,
    Phishing,
    Malware,
    Scam,
    Promotional,
    Inappropriate
}

/// <summary>
/// Emotion types
/// </summary>
public enum EmotionType
{
    Happy,
    Sad,
    Angry,
    Fear,
    Surprise,
    Disgust,
    Neutral,
    Contempt
}

/// <summary>
/// Gender classification
/// </summary>
public enum Gender
{
    Male,
    Female,
    Unknown
}

/// <summary>
/// Entity types for extraction
/// </summary>
public enum EntityType
{
    Person,
    Organization,
    Location,
    Date,
    Money,
    Percent,
    PhoneNumber,
    Email,
    URL,
    Product,
    Event,
    DateTime
}

/// <summary>
/// Content types for ML processing
/// </summary>
public enum ContentType
{
    Image,
    Text,
    Video,
    Audio,
    Document
}

/// <summary>
/// ML operations for batch processing
/// </summary>
public enum MLOperation
{
    TagGeneration,
    ContentModeration,
    QualityAnalysis,
    ObjectDetection,
    ColorAnalysis,
    FaceDetection,
    GenerateTags,
    ModerateContent,
    AnalyzeQuality,
    DetectObjects,
    AnalyzeColors,
    DetectFaces,
    GenerateAltText
}