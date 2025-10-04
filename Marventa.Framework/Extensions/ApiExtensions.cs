using Marventa.Framework.Configuration;
using Marventa.Framework.Core.Shared;
using Marventa.Framework.Infrastructure.Environment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Provides extension methods for configuring API-related services.
/// Includes Swagger/OpenAPI, API versioning, and CORS configuration.
/// </summary>
public static class ApiExtensions
{
    /// <summary>
    /// Adds API versioning with configurable strategy (URL segment, query string, header, or media type).
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaApiVersioning(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.IsSectionEnabled(ConfigurationKeys.ApiVersioning))
        {
            return services;
        }

        var options = configuration.GetConfigurationSection<ApiVersioningOptions>(
            ApiVersioningOptions.SectionName);

        services.Configure<ApiVersioningOptions>(
            configuration.GetSection(ApiVersioningOptions.SectionName));

        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = ApiVersion.Parse(options.DefaultVersion);
            config.AssumeDefaultVersionWhenUnspecified = options.AssumeDefaultVersionWhenUnspecified;
            config.ReportApiVersions = options.ReportApiVersions;

            // Configure versioning strategy
            config.ApiVersionReader = options.VersioningType switch
            {
                VersioningType.QueryString => new Microsoft.AspNetCore.Mvc.Versioning.QueryStringApiVersionReader(
                    options.QueryStringParameterName),
                VersioningType.Header => new Microsoft.AspNetCore.Mvc.Versioning.HeaderApiVersionReader(
                    options.HeaderName),
                VersioningType.MediaType => new Microsoft.AspNetCore.Mvc.Versioning.MediaTypeApiVersionReader(),
                VersioningType.UrlSegment or _ => new Microsoft.AspNetCore.Mvc.Versioning.UrlSegmentApiVersionReader()
            };
        });

        services.AddVersionedApiExplorer(explorerOptions =>
        {
            explorerOptions.GroupNameFormat = "'v'VVV";
            explorerOptions.SubstituteApiVersionInUrl = options.VersioningType == VersioningType.UrlSegment;
        });

        return services;
    }

    /// <summary>
    /// Adds Swagger/OpenAPI documentation with support for API versioning and JWT authentication.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaSwagger(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.IsSectionEnabled(ConfigurationKeys.Swagger))
        {
            return services;
        }

        var swaggerOptions = configuration.GetConfigurationSection<SwaggerOptions>(
            SwaggerOptions.SectionName);

        services.Configure<SwaggerOptions>(
            configuration.GetSection(SwaggerOptions.SectionName));

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            // Configure Swagger documents for API versioning
            if (configuration.HasSection(ConfigurationKeys.ApiVersioning))
            {
                var serviceProvider = services.BuildServiceProvider();
                var apiVersionProvider = serviceProvider.GetService<IApiVersionDescriptionProvider>();

                if (apiVersionProvider != null)
                {
                    foreach (var description in apiVersionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(
                            description.GroupName,
                            CreateOpenApiInfo(swaggerOptions, description.ApiVersion.ToString()));
                    }
                }
                else
                {
                    options.SwaggerDoc(swaggerOptions.Version, CreateOpenApiInfo(swaggerOptions));
                }

                serviceProvider.Dispose();
            }
            else
            {
                options.SwaggerDoc(swaggerOptions.Version, CreateOpenApiInfo(swaggerOptions));
            }

            // Configure JWT Bearer authentication
            if (swaggerOptions.RequireAuthorization && configuration.HasSection(ConfigurationKeys.Jwt))
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. " +
                                "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                                "Example: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        Array.Empty<string>()
                    }
                });
            }

            // Include XML documentation files
            var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var xmlFile in xmlFiles)
            {
                options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
            }
        });

        return services;
    }

    /// <summary>
    /// Adds CORS (Cross-Origin Resource Sharing) policy based on allowed origins from configuration.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection(ConfigurationKeys.CorsAllowedOrigins)
            .Get<string[]>();

        if (allowedOrigins == null || allowedOrigins.Length == 0)
        {
            return services;
        }

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder =>
            {
                // "*" için AllowAnyOrigin kullan (AllowCredentials ile çakışır)
                if (allowedOrigins.Length == 1 && allowedOrigins[0] == CorsConstants.AllowAllOrigins)
                {
                    policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
                else
                {
                    policyBuilder
                        .WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Adds all API-related services including versioning, Swagger, and CORS.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMarventaApiVersioning(configuration);
        services.AddMarventaSwagger(configuration);
        services.AddMarventaCors(configuration);

        return services;
    }

    /// <summary>
    /// Creates OpenAPI info from Swagger options.
    /// </summary>
    private static OpenApiInfo CreateOpenApiInfo(SwaggerOptions options, string? version = null)
    {
        return new OpenApiInfo
        {
            Title = options.Title,
            Version = version ?? options.Version,
            Description = options.Description,
            Contact = options.Contact != null
                ? new OpenApiContact
                {
                    Name = options.Contact.Name,
                    Email = options.Contact.Email,
                    Url = string.IsNullOrWhiteSpace(options.Contact.Url)
                        ? null
                        : new Uri(options.Contact.Url)
                }
                : null,
            License = options.License != null
                ? new OpenApiLicense
                {
                    Name = options.License.Name,
                    Url = string.IsNullOrWhiteSpace(options.License.Url)
                        ? null
                        : new Uri(options.License.Url)
                }
                : null
        };
    }
}
