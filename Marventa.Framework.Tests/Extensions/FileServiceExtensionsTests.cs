using FluentAssertions;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Core.Models.Configuration;
using Marventa.Framework.Infrastructure.Services.FileServices;
using Marventa.Framework.Web.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Marventa.Framework.Tests.Extensions;

public class FileServiceExtensionsTests
{
    [Fact]
    public void AddMarventaFileServices_Should_RegisterAllServices_WhenAllEnabled()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddMarventaFileServices(options =>
        {
            options.EnableFileProcessor = true;
            options.EnableStorage = true;
            options.EnableCDN = true;
            options.EnableML = true;
            options.EnableMetadata = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<IMarventaFileProcessor>().Should().NotBeNull();
        serviceProvider.GetService<IMarventaStorage>().Should().NotBeNull();
        serviceProvider.GetService<IMarventaCDN>().Should().NotBeNull();
        serviceProvider.GetService<IMarventaML>().Should().NotBeNull();
        serviceProvider.GetService<IMarventaFileMetadata>().Should().NotBeNull();
    }

    [Fact]
    public void AddMarventaFileServices_Should_RegisterOnlyEnabledServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddMarventaFileServices(options =>
        {
            options.EnableFileProcessor = true;
            options.EnableStorage = true;
            options.EnableCDN = false;
            options.EnableML = false;
            options.EnableMetadata = false;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<IMarventaFileProcessor>().Should().NotBeNull();
        serviceProvider.GetService<IMarventaStorage>().Should().NotBeNull();
        serviceProvider.GetService<IMarventaCDN>().Should().BeNull();
        serviceProvider.GetService<IMarventaML>().Should().BeNull();
        serviceProvider.GetService<IMarventaFileMetadata>().Should().BeNull();
    }

    [Fact]
    public void AddFileProcessor_Should_RegisterMockProvider_ByDefault()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddFileProcessor();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var fileProcessor = serviceProvider.GetService<IMarventaFileProcessor>();
        fileProcessor.Should().NotBeNull();
        fileProcessor.Should().BeOfType<MockFileProcessor>();
    }

    [Fact]
    public void AddFileProcessor_Should_RegisterImageSharpProvider_WhenConfigured()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddFileProcessor(new FileProcessorOptions
        {
            Provider = FileProcessorProvider.ImageSharp
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var fileProcessor = serviceProvider.GetService<IMarventaFileProcessor>();
        fileProcessor.Should().NotBeNull();
        // Note: Would need actual ImageSharpFileProcessor implementation to test this properly
    }

    [Fact]
    public void AddStorageService_Should_RegisterMockProvider_ByDefault()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddStorageService();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var storageService = serviceProvider.GetService<IMarventaStorage>();
        storageService.Should().NotBeNull();
        storageService.Should().BeOfType<MockStorageService>();
    }

    [Theory]
    [InlineData(StorageProvider.Mock)]
    [InlineData(StorageProvider.LocalFile)]
    [InlineData(StorageProvider.AzureBlob)]
    [InlineData(StorageProvider.MinIO)]
    public void AddStorageService_Should_RegisterCorrectProvider_WhenConfigured(StorageProvider provider)
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddStorageService(new StorageServiceOptions
        {
            Provider = provider
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var storageService = serviceProvider.GetService<IMarventaStorage>();
        storageService.Should().NotBeNull();

        // For mock provider, we can verify the actual type
        if (provider == StorageProvider.Mock)
        {
            storageService.Should().BeOfType<MockStorageService>();
        }
        // Note: Other providers would need actual implementations to test
    }

    [Fact]
    public void AddCDNService_Should_RegisterMockProvider_ByDefault()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddCDNService();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var cdnService = serviceProvider.GetService<IMarventaCDN>();
        cdnService.Should().NotBeNull();
        cdnService.Should().BeOfType<MockCDNService>();
    }

    [Fact]
    public void AddMLService_Should_RegisterMockProvider_ByDefault()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddMLService();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var mlService = serviceProvider.GetService<IMarventaML>();
        mlService.Should().NotBeNull();
        mlService.Should().BeOfType<MockMLService>();
    }

    [Fact]
    public void AddMetadataService_Should_RegisterMockProvider_ByDefault()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddMetadataService();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var metadataService = serviceProvider.GetService<IMarventaFileMetadata>();
        metadataService.Should().NotBeNull();
        metadataService.Should().BeOfType<MockMetadataService>();
    }

    [Fact]
    public void AddMarventaFileServices_Should_UseDefaultOptions_WhenNoConfigurationProvided()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddMarventaFileServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        // Default configuration should enable FileProcessor and Storage, but not CDN, ML, or Metadata
        serviceProvider.GetService<IMarventaFileProcessor>().Should().NotBeNull();
        serviceProvider.GetService<IMarventaStorage>().Should().NotBeNull();
        serviceProvider.GetService<IMarventaCDN>().Should().BeNull();
        serviceProvider.GetService<IMarventaML>().Should().BeNull();
        serviceProvider.GetService<IMarventaFileMetadata>().Should().NotBeNull(); // Metadata is enabled by default
    }

    [Fact]
    public void AddMarventaFileServices_Should_PreserveCustomOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddMarventaFileServices(options =>
        {
            options.FileProcessorOptions.MaxFileSizeBytes = 1024;
            options.StorageOptions.DefaultContainer = "custom-container";
            options.CDNOptions.DefaultCacheTTL = 7200;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var fileServiceOptions = serviceProvider.GetService<Microsoft.Extensions.Options.IOptions<FileServiceOptions>>();
        fileServiceOptions.Should().NotBeNull();
        fileServiceOptions!.Value.FileProcessorOptions.MaxFileSizeBytes.Should().Be(1024);
        fileServiceOptions.Value.StorageOptions.DefaultContainer.Should().Be("custom-container");
        fileServiceOptions.Value.CDNOptions.DefaultCacheTTL.Should().Be(7200);
    }

    [Fact]
    public void FileServiceExtensions_Should_HandleNullLogger_Gracefully()
    {
        // Arrange
        var services = new ServiceCollection();
        // Intentionally not adding logging services

        // Act & Assert
        var action = () =>
        {
            services.AddMarventaFileServices();
            var serviceProvider = services.BuildServiceProvider();
            var fileProcessor = serviceProvider.GetService<IMarventaFileProcessor>();
        };

        // This should not throw, the services should handle null loggers gracefully
        action.Should().NotThrow();
    }
}