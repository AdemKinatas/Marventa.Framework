using Marventa.Framework.Core.Models.ML;

namespace Marventa.Framework.Core.Interfaces;

/// <summary>
/// Provides machine learning capabilities for content analysis, moderation, and optimization
/// </summary>
public interface IMarventaML
{
    #region Image Analysis

    /// <summary>
    /// Generates descriptive tags for an image using computer vision
    /// </summary>
    /// <param name="image">Input image stream</param>
    /// <param name="maxTags">Maximum number of tags to generate</param>
    /// <param name="confidence">Minimum confidence threshold (0.0 to 1.0)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Array of generated tags with confidence scores</returns>
    Task<TagGenerationResult> GenerateTagsAsync(Stream image, int maxTags = 10, double confidence = 0.7, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes content for inappropriate material (violence, adult content, etc.)
    /// </summary>
    /// <param name="image">Input image stream</param>
    /// <param name="categories">Specific categories to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Moderation result with safety scores</returns>
    Task<ModerationResult> ModerateContentAsync(Stream image, ModerationCategory[]? categories = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes technical quality aspects of an image (blur, noise, exposure, etc.)
    /// </summary>
    /// <param name="image">Input image stream</param>
    /// <param name="analysisOptions">Quality analysis configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Comprehensive quality score and metrics</returns>
    Task<QualityAnalysisResult> AnalyzeQualityAsync(Stream image, QualityAnalysisOptions? analysisOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects and identifies objects within an image with bounding boxes
    /// </summary>
    /// <param name="image">Input image stream</param>
    /// <param name="confidence">Minimum confidence threshold</param>
    /// <param name="maxObjects">Maximum number of objects to detect</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Array of detected objects with locations and confidence</returns>
    Task<ObjectDetectionResult> DetectObjectsAsync(Stream image, double confidence = 0.5, int maxObjects = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes dominant colors and color palette of an image
    /// </summary>
    /// <param name="image">Input image stream</param>
    /// <param name="maxColors">Maximum number of colors to extract</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Color analysis with palette and distribution</returns>
    Task<ColorAnalysisResult> AnalyzeColorsAsync(Stream image, int maxColors = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects faces in an image with optional emotion and demographic analysis
    /// </summary>
    /// <param name="image">Input image stream</param>
    /// <param name="includeEmotions">Whether to analyze emotions</param>
    /// <param name="includeDemographics">Whether to estimate age/gender</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Face detection result with optional additional analysis</returns>
    Task<FaceDetectionResult> DetectFacesAsync(Stream image, bool includeEmotions = false, bool includeDemographics = false, CancellationToken cancellationToken = default);

    #endregion

    #region Text Analysis

    /// <summary>
    /// Analyzes the sentiment of text content
    /// </summary>
    /// <param name="text">Input text to analyze</param>
    /// <param name="language">Language code (auto-detect if null)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sentiment analysis with scores and confidence</returns>
    Task<SentimentAnalysisResult> AnalyzeSentimentAsync(string text, string? language = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts important keywords and phrases from text
    /// </summary>
    /// <param name="text">Input text to analyze</param>
    /// <param name="maxKeywords">Maximum number of keywords to extract</param>
    /// <param name="language">Language code (auto-detect if null)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Extracted keywords with relevance scores</returns>
    Task<KeywordExtractionResult> ExtractKeywordsAsync(string text, int maxKeywords = 20, string? language = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects spam, phishing, or other malicious content in text
    /// </summary>
    /// <param name="text">Input text to analyze</param>
    /// <param name="checkTypes">Types of threats to check for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Spam detection result with threat analysis</returns>
    Task<SpamDetectionResult> DetectSpamAsync(string text, SpamCheckType[]? checkTypes = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Classifies text into predefined categories
    /// </summary>
    /// <param name="text">Input text to classify</param>
    /// <param name="categories">Available categories</param>
    /// <param name="language">Language code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Classification result with category probabilities</returns>
    Task<TextClassificationResult> ClassifyTextAsync(string text, string[] categories, string? language = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts entities (people, places, organizations) from text
    /// </summary>
    /// <param name="text">Input text to analyze</param>
    /// <param name="entityTypes">Types of entities to extract</param>
    /// <param name="language">Language code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Named entity recognition result</returns>
    Task<EntityExtractionResult> ExtractEntitiesAsync(string text, EntityType[]? entityTypes = null, string? language = null, CancellationToken cancellationToken = default);

    #endregion

    #region Content Optimization

    /// <summary>
    /// Generates SEO-optimized alt text for images
    /// </summary>
    /// <param name="image">Input image stream</param>
    /// <param name="context">Additional context about the image</param>
    /// <param name="maxLength">Maximum alt text length</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated alt text with SEO optimization</returns>
    Task<AltTextGenerationResult> GenerateAltTextAsync(Stream image, string? context = null, int maxLength = 125, CancellationToken cancellationToken = default);

    /// <summary>
    /// Suggests content improvements based on analysis
    /// </summary>
    /// <param name="content">Content to analyze (text or image)</param>
    /// <param name="contentType">Type of content</param>
    /// <param name="targetAudience">Target audience description</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Content optimization suggestions</returns>
    Task<ContentOptimizationResult> SuggestOptimizationsAsync(Stream content, ContentType contentType, string? targetAudience = null, CancellationToken cancellationToken = default);

    #endregion

    #region Batch Operations

    /// <summary>
    /// Processes multiple images in batch for improved efficiency
    /// </summary>
    /// <param name="images">Dictionary of image streams with identifiers</param>
    /// <param name="operations">ML operations to perform</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Batch processing results</returns>
    Task<BatchProcessingResult> ProcessBatchAsync(Dictionary<string, Stream> images, MLOperation[] operations, CancellationToken cancellationToken = default);

    #endregion
}