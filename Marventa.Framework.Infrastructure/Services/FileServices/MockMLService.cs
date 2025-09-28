using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Core.Models.ML;
using Microsoft.Extensions.Logging;

namespace Marventa.Framework.Infrastructure.Services.FileServices;

/// <summary>
/// Mock implementation of ML service for development and testing
/// </summary>
public class MockMLService : IMarventaML
{
    private readonly ILogger<MockMLService> _logger;
    private readonly Random _random = new();

    public MockMLService(ILogger<MockMLService> logger)
    {
        _logger = logger;
    }

    public Task<TagGenerationResult> GenerateTagsAsync(Stream image, int maxTags = 10, double confidence = 0.7, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Generating tags for image (max: {MaxTags}, confidence: {Confidence})", maxTags, confidence);

        var mockTags = new[]
        {
            "landscape", "nature", "outdoor", "scenic", "mountains", "forest", "lake", "sunset",
            "people", "person", "family", "friends", "portrait", "smile", "happy",
            "city", "urban", "building", "architecture", "street", "car", "traffic",
            "food", "meal", "restaurant", "cooking", "delicious", "fresh",
            "animal", "pet", "dog", "cat", "wildlife", "bird", "cute"
        };

        var tags = mockTags
            .OrderBy(_ => _random.Next())
            .Take(Math.Min(maxTags, mockTags.Length))
            .Select(tag => new ImageTag
            {
                Name = tag,
                Confidence = Math.Round(_random.NextDouble() * (1.0 - confidence) + confidence, 3),
                BoundingBox = _random.NextDouble() > 0.7 ? new BoundingBox
                {
                    X = _random.Next(0, 800),
                    Y = _random.Next(0, 600),
                    Width = _random.Next(100, 300),
                    Height = _random.Next(100, 300)
                } : null
            })
            .ToArray();

        var result = new TagGenerationResult
        {
            Tags = tags,
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(100, 500)),
            ModelVersion = "mock-vision-v1.0",
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<ModerationResult> ModerateContentAsync(Stream image, ModerationCategory[]? categories = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Moderating content");

        var scores = new Dictionary<ModerationCategory, double>
        {
            [ModerationCategory.Adult] = _random.NextDouble() * 0.3, // Low scores for safe content
            [ModerationCategory.Violence] = _random.NextDouble() * 0.2,
            [ModerationCategory.Hate] = _random.NextDouble() * 0.1,
            [ModerationCategory.Drugs] = _random.NextDouble() * 0.1,
            [ModerationCategory.Spam] = _random.NextDouble() * 0.15
        };

        var result = new ModerationResult
        {
            IsAppropriate = scores.Values.All(score => score < 0.5),
            OverallRiskScore = scores.Values.Max(),
            CategoryScores = scores,
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(50, 200)),
            ModelVersion = "mock-moderation-v1.0",
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<QualityAnalysisResult> AnalyzeQualityAsync(Stream image, QualityAnalysisOptions? analysisOptions = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Analyzing image quality");

        var result = new QualityAnalysisResult
        {
            OverallQualityScore = Math.Round(_random.NextDouble() * 0.4 + 0.6, 3), // 0.6-1.0 range
            BlurScore = Math.Round(_random.NextDouble() * 0.3, 3), // Lower is better
            NoiseScore = Math.Round(_random.NextDouble() * 0.25, 3),
            ExposureScore = Math.Round(_random.NextDouble() * 0.8 + 0.2, 3), // 0.2-1.0 range
            ContrastScore = Math.Round(_random.NextDouble() * 0.6 + 0.4, 3),
            SharpnessScore = Math.Round(_random.NextDouble() * 0.5 + 0.5, 3),
            ColorBalance = Math.Round(_random.NextDouble() * 0.4 + 0.6, 3),
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(200, 800)),
            Recommendations = new[] { "Image quality is good", "Consider slight contrast adjustment" },
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<ObjectDetectionResult> DetectObjectsAsync(Stream image, double confidence = 0.5, int maxObjects = 50, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Detecting objects in image");

        var mockObjects = new[]
        {
            "person", "car", "truck", "bus", "motorcycle", "bicycle", "dog", "cat", "bird",
            "tree", "building", "sign", "traffic light", "bench", "table", "chair"
        };

        var objects = mockObjects
            .OrderBy(_ => _random.Next())
            .Take(_random.Next(1, Math.Min(maxObjects, 10)))
            .Select(obj => new DetectedObject
            {
                Label = obj,
                Confidence = Math.Round(_random.NextDouble() * (1.0 - confidence) + confidence, 3),
                BoundingBox = new BoundingBox
                {
                    X = _random.Next(0, 800),
                    Y = _random.Next(0, 600),
                    Width = _random.Next(50, 200),
                    Height = _random.Next(50, 200)
                }
            })
            .ToArray();

        var result = new ObjectDetectionResult
        {
            Objects = objects,
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(300, 1000)),
            ModelVersion = "mock-detection-v1.0",
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<ColorAnalysisResult> AnalyzeColorsAsync(Stream image, int maxColors = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Analyzing colors in image");

        var colors = Enumerable.Range(0, Math.Min(maxColors, 8))
            .Select(_ => new DominantColor
            {
                Hex = $"#{_random.Next(0x1000000):X6}",
                RGB = new RGBColor
                {
                    R = _random.Next(0, 256),
                    G = _random.Next(0, 256),
                    B = _random.Next(0, 256)
                },
                Percentage = Math.Round(_random.NextDouble() * 30, 2)
            })
            .ToArray();

        // Normalize percentages
        var total = colors.Sum(c => c.Percentage);
        foreach (var color in colors)
        {
            color.Percentage = Math.Round(color.Percentage / total * 100, 2);
        }

        var result = new ColorAnalysisResult
        {
            DominantColors = colors,
            ColorPalette = colors.Select(c => c.Hex).ToArray(),
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(100, 300)),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<FaceDetectionResult> DetectFacesAsync(Stream image, bool includeEmotions = false, bool includeDemographics = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Detecting faces in image");

        var faceCount = _random.Next(0, 4); // 0-3 faces
        var faces = Enumerable.Range(0, faceCount)
            .Select(_ => new DetectedFace
            {
                BoundingBox = new BoundingBox
                {
                    X = _random.Next(0, 600),
                    Y = _random.Next(0, 400),
                    Width = _random.Next(80, 150),
                    Height = _random.Next(80, 150)
                },
                Confidence = Math.Round(_random.NextDouble() * 0.4 + 0.6, 3),
                Emotions = includeEmotions ? new Dictionary<string, double>
                {
                    ["happy"] = _random.NextDouble(),
                    ["sad"] = _random.NextDouble() * 0.3,
                    ["angry"] = _random.NextDouble() * 0.2,
                    ["surprised"] = _random.NextDouble() * 0.4,
                    ["neutral"] = _random.NextDouble() * 0.6
                } : null,
                Demographics = includeDemographics ? new FaceDemographics
                {
                    AgeRange = new AgeRange { Min = _random.Next(18, 30), Max = _random.Next(30, 70) },
                    Gender = _random.NextDouble() > 0.5 ? "male" : "female",
                    GenderConfidence = _random.NextDouble() * 0.4 + 0.6
                } : null
            })
            .ToArray();

        var result = new FaceDetectionResult
        {
            Faces = faces,
            // FaceCount is a calculated property
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(200, 600)),
            ModelVersion = "mock-face-v1.0",
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<SentimentAnalysisResult> AnalyzeSentimentAsync(string text, string? language = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Analyzing sentiment for text of length {Length}", text.Length);

        var sentiments = new[] { "positive", "negative", "neutral" };
        var sentiment = sentiments[_random.Next(sentiments.Length)];

        var scores = new Dictionary<string, double>
        {
            ["positive"] = sentiment == "positive" ? _random.NextDouble() * 0.4 + 0.6 : _random.NextDouble() * 0.4,
            ["negative"] = sentiment == "negative" ? _random.NextDouble() * 0.4 + 0.6 : _random.NextDouble() * 0.4,
            ["neutral"] = sentiment == "neutral" ? _random.NextDouble() * 0.4 + 0.6 : _random.NextDouble() * 0.4
        };

        var result = new SentimentAnalysisResult
        {
            OverallSentiment = sentiment,
            SentimentScores = scores,
            Confidence = scores[sentiment],
            Language = language ?? "en",
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(50, 200)),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<KeywordExtractionResult> ExtractKeywordsAsync(string text, int maxKeywords = 20, string? language = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Extracting keywords from text");

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 3)
            .Distinct()
            .Take(maxKeywords)
            .Select(word => new ExtractedKeyword
            {
                Text = word.ToLower(),
                Relevance = Math.Round(_random.NextDouble(), 3),
                Frequency = _random.Next(1, 10)
            })
            .OrderByDescending(k => k.Relevance)
            .ToArray();

        var result = new KeywordExtractionResult
        {
            Keywords = words,
            Language = language ?? "en",
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(100, 300)),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<SpamDetectionResult> DetectSpamAsync(string text, SpamCheckType[]? checkTypes = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Detecting spam in text");

        var spamScore = _random.NextDouble() * 0.3; // Generally low spam scores
        var isSpam = spamScore > 0.5;

        var result = new SpamDetectionResult
        {
            IsSpam = isSpam,
            SpamScore = Math.Round(spamScore, 3),
            DetectedThreats = isSpam ? new[] { "promotional_content" } : Array.Empty<string>(),
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(50, 150)),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<TextClassificationResult> ClassifyTextAsync(string text, string[] categories, string? language = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Classifying text into {Count} categories", categories.Length);

        var scores = categories.ToDictionary(
            category => category,
            _ => Math.Round(_random.NextDouble(), 3));

        var topCategory = scores.OrderByDescending(kvp => kvp.Value).First();

        var result = new TextClassificationResult
        {
            TopCategory = topCategory.Key,
            CategoryScores = scores,
            Confidence = topCategory.Value,
            Language = language ?? "en",
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(100, 250)),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<EntityExtractionResult> ExtractEntitiesAsync(string text, EntityType[]? entityTypes = null, string? language = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Extracting entities from text");

        var mockEntities = new[]
        {
            new ExtractedEntity { Text = "John Doe", Type = EntityType.Person, Confidence = 0.95, StartIndex = 0, EndIndex = 8 },
            new ExtractedEntity { Text = "Microsoft", Type = EntityType.Organization, Confidence = 0.89, StartIndex = 20, EndIndex = 29 },
            new ExtractedEntity { Text = "Seattle", Type = EntityType.Location, Confidence = 0.92, StartIndex = 35, EndIndex = 42 },
            new ExtractedEntity { Text = "2024-01-15", Type = EntityType.DateTime, Confidence = 0.98, StartIndex = 50, EndIndex = 60 }
        };

        var filteredEntities = entityTypes != null
            ? mockEntities.Where(e => entityTypes.Contains(e.Type)).ToArray()
            : mockEntities;

        var result = new EntityExtractionResult
        {
            Entities = filteredEntities.Take(_random.Next(1, filteredEntities.Length + 1)).ToArray(),
            Language = language ?? "en",
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(150, 400)),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<AltTextGenerationResult> GenerateAltTextAsync(Stream image, string? context = null, int maxLength = 125, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Generating alt text for image");

        var altTexts = new[]
        {
            "A beautiful landscape with mountains and a lake at sunset",
            "Group of people sitting around a table having a meeting",
            "Modern city skyline with tall buildings and busy street",
            "Close-up of a delicious meal on a wooden table",
            "Cute dog playing in a park with green grass and trees"
        };

        var altText = altTexts[_random.Next(altTexts.Length)];
        if (altText.Length > maxLength)
        {
            altText = altText.Substring(0, maxLength - 3) + "...";
        }

        var result = new AltTextGenerationResult
        {
            AltText = altText,
            Confidence = Math.Round(_random.NextDouble() * 0.3 + 0.7, 3),
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(200, 500)),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<ContentOptimizationResult> SuggestOptimizationsAsync(Stream content, ContentType contentType, string? targetAudience = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Suggesting optimizations for {ContentType} content", contentType);

        var suggestionTexts = new[]
        {
            "Consider reducing file size for faster loading",
            "Add more descriptive keywords for better SEO",
            "Improve contrast for better accessibility",
            "Optimize for mobile viewing",
            "Add captions for hearing-impaired users"
        };

        var suggestions = suggestionTexts.Take(_random.Next(2, suggestionTexts.Length))
            .Select(s => new OptimizationSuggestion
            {
                Category = "General",
                Suggestion = s,
                Impact = _random.NextDouble(),
                Priority = "Medium"
            })
            .ToArray();

        var result = new ContentOptimizationResult
        {
            Suggestions = suggestions,
            OverallScore = Math.Round(_random.NextDouble() * 0.4 + 0.6, 3),
            TargetAudience = targetAudience ?? "general",
            ProcessingTime = TimeSpan.FromMilliseconds(_random.Next(300, 700)),
            Success = true
        };

        return Task.FromResult(result);
    }

    public Task<BatchProcessingResult> ProcessBatchAsync(Dictionary<string, Stream> images, MLOperation[] operations, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Processing batch of {Count} images with {OperationCount} operations", images.Count, operations.Length);

        var results = new Dictionary<string, Dictionary<MLOperation, object>>();

        foreach (var (imageId, stream) in images)
        {
            var imageResults = new Dictionary<MLOperation, object>();

            foreach (var operation in operations)
            {
                object result = operation switch
                {
                    MLOperation.GenerateTags => GenerateTagsAsync(stream).Result,
                    MLOperation.ModerateContent => ModerateContentAsync(stream).Result,
                    MLOperation.AnalyzeQuality => AnalyzeQualityAsync(stream).Result,
                    MLOperation.DetectObjects => DetectObjectsAsync(stream).Result,
                    MLOperation.AnalyzeColors => AnalyzeColorsAsync(stream).Result,
                    MLOperation.DetectFaces => DetectFacesAsync(stream).Result,
                    MLOperation.GenerateAltText => GenerateAltTextAsync(stream).Result,
                    _ => new { Success = true, Message = "Operation completed" }
                };

                imageResults[operation] = result;
            }

            results[imageId] = imageResults;
        }

        var batchResult = new BatchProcessingResult
        {
            TotalImages = images.Count,
            SuccessfulProcessing = images.Count,
            FailedProcessing = 0,
            Results = results,
            ProcessingTime = TimeSpan.FromMilliseconds(images.Count * operations.Length * 100),
            Success = true
        };

        return Task.FromResult(batchResult);
    }
}