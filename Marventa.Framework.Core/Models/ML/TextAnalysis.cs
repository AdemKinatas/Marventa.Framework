namespace Marventa.Framework.Core.Models.ML;

/// <summary>
/// Sentiment analysis result
/// </summary>
public class SentimentAnalysisResult
{
    /// <summary>
    /// Overall sentiment
    /// </summary>
    public SentimentType Sentiment { get; set; }

    /// <summary>
    /// Overall sentiment as string
    /// </summary>
    public string OverallSentiment { get; set; } = string.Empty;

    /// <summary>
    /// Sentiment scores by type
    /// </summary>
    public Dictionary<string, double> SentimentScores { get; set; } = new();

    /// <summary>
    /// Confidence in sentiment classification
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Sentiment scores by type
    /// </summary>
    public Dictionary<SentimentType, double> Scores { get; set; } = new();

    /// <summary>
    /// Detected language
    /// </summary>
    public string DetectedLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Language used for analysis
    /// </summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Whether analysis was successful
    /// </summary>
    public bool Success { get; set; }
}

/// <summary>
/// Keyword extraction result
/// </summary>
public class KeywordExtractionResult
{
    /// <summary>
    /// Extracted keywords with relevance scores
    /// </summary>
    public ExtractedKeyword[] Keywords { get; set; } = Array.Empty<ExtractedKeyword>();

    /// <summary>
    /// Extracted phrases
    /// </summary>
    public KeyPhrase[] Phrases { get; set; } = Array.Empty<KeyPhrase>();

    /// <summary>
    /// Detected language
    /// </summary>
    public string DetectedLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Language used for analysis
    /// </summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Whether extraction was successful
    /// </summary>
    public bool Success { get; set; }
}

/// <summary>
/// Extracted keyword with metadata
/// </summary>
public class ExtractedKeyword
{
    public string Text { get; set; } = string.Empty;
    public double Relevance { get; set; }
    public int Frequency { get; set; }
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Extracted keyword with metadata
/// </summary>
public class Keyword
{
    public string Term { get; set; } = string.Empty;
    public double Relevance { get; set; }
    public int Frequency { get; set; }
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Extracted key phrase
/// </summary>
public class KeyPhrase
{
    public string Phrase { get; set; } = string.Empty;
    public double Relevance { get; set; }
    public int Position { get; set; }
}

/// <summary>
/// Spam detection result
/// </summary>
public class SpamDetectionResult
{
    /// <summary>
    /// Whether content is likely spam
    /// </summary>
    public bool IsSpam { get; set; }

    /// <summary>
    /// Spam probability (0.0 to 1.0)
    /// </summary>
    public double SpamProbability { get; set; }

    /// <summary>
    /// Spam score
    /// </summary>
    public double SpamScore { get; set; }

    /// <summary>
    /// Detected spam threats
    /// </summary>
    public string[] DetectedThreats { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Detected spam indicators
    /// </summary>
    public SpamIndicator[] Indicators { get; set; } = Array.Empty<SpamIndicator>();

    /// <summary>
    /// Risk assessment by category
    /// </summary>
    public Dictionary<SpamCheckType, double> RiskScores { get; set; } = new();

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Processing time as TimeSpan
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }

    /// <summary>
    /// Whether detection was successful
    /// </summary>
    public bool Success { get; set; }
}

/// <summary>
/// Spam indicator details
/// </summary>
public class SpamIndicator
{
    public SpamCheckType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Evidence { get; set; } = string.Empty;
}

/// <summary>
/// Text classification result
/// </summary>
public class TextClassificationResult
{
    public Dictionary<string, double> CategoryProbabilities { get; set; } = new();
    public Dictionary<string, double> CategoryScores { get; set; } = new();
    public string PredictedCategory { get; set; } = string.Empty;
    public string TopCategory { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string DetectedLanguage { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public long ProcessingTimeMs { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public bool Success { get; set; }
}

/// <summary>
/// Entity extraction result
/// </summary>
public class EntityExtractionResult
{
    public ExtractedEntity[] Entities { get; set; } = Array.Empty<ExtractedEntity>();
    public string DetectedLanguage { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public long ProcessingTimeMs { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public bool Success { get; set; }
}

/// <summary>
/// Extracted entity information
/// </summary>
public class ExtractedEntity
{
    public string Text { get; set; } = string.Empty;
    public EntityType Type { get; set; }
    public double Confidence { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}