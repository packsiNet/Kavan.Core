using ApplicationLayer.Common.Behaviors;
using ApplicationLayer.Features.Validations;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Binance;
using ApplicationLayer.Mapping.UserAccounts;
using ApplicationLayer.Mapping.MarketAnalysis;
using ApplicationLayer.Services;
using AspNetCoreRateLimit;
using DomainLayer.Common.Attributes;
using FluentValidation;
using InfrastructureLayer.BusinessLogic.Services;
using InfrastructureLayer.Context;
using InfrastructureLayer.Extensions;
using InfrastructureLayer.Repository;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace InfrastructureLayer;

public static class DependencyInjection
{
    public static IServiceCollection Register(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterService(configuration);

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserContextService, UserContextService>();

        services.AddSingleton<IApplicationBuilder, ApplicationBuilder>();
        services.AddScoped<ICandleUpdaterService, CandleUpdaterService>();
        services.AddScoped<ICandleAggregatorService, CandleAggregatorService>();

        // اجرای سرویس پس‌زمینه در همه محیط‌ها
        // Temporarily commented out to fix DI issue and for faster startup during testing
        // services.AddHostedService<CandleUpdaterHostedService>();
        // services.AddHostedService<CandleAggregatorHostedService>();
        // services.AddHostedService<TechnicalSignalBackgroundService>();

        services.AddHttpContextAccessor();
        services.MediatRDependency();
        services.RegisterServicesAutomatically();
        services.FluentValidationConfiguration();
        services.RateLimitingConfiguration(configuration);
        services.AddCors(opt => opt.AddPolicy("AllowSpecificOrigin", builder =>
        {
            builder.WithOrigins("https://tg.kavan.trade", "https://core.kavan.trade", "https://panel.kavan.trade", "http://localhost:3000", "http://localhost:5173", "http://localhost:8080", "http://127.0.0.1:3000", "http://127.0.0.1:5173", "http://127.0.0.1:8080")
            .SetIsOriginAllowed(origin => true) // Allow any origin for development
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        }));

        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365); // 1 year
        });
        return services;
    }

    private static void RegisterService(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigurationDependency();
        services.AddMemoryCache();
        services.SwaggerConfiguration(configuration);
        services.JwtAuthorizeConfiguration(configuration);
        services.SeriLogConfiguration(configuration);
        services.AddAutoMapper(typeof(UserAccountProfile).Assembly);
    }

    private static void SeriLogConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var columnOptions = new ColumnOptions();
        // Define additional custom columns if needed
        //columnOptions.AdditionalColumns = new Collection<SqlColumn>
        //{
        //    new SqlColumn { ColumnName = "CustomColumn1", DataType = System.Data.SqlDbType.NVarChar, DataLength = 100 },
        //    new SqlColumn { ColumnName = "CustomColumn2", DataType = System.Data.SqlDbType.Int }
        //};

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .WriteTo.MSSqlServer(
                connectionString: configuration.GetConnectionString("LogConnection"),
                sinkOptions: new MSSqlServerSinkOptions
                {
                    TableName = configuration.GetSection("Serilog:TableName").Value,
                    SchemaName = configuration.GetSection("Serilog:SchemaName").Value,
                    AutoCreateSqlTable = true
                },
                columnOptions: columnOptions
            )
            .CreateLogger();

        services.AddSingleton(Log.Logger);
        services.AddLogging(loggingBuilder =>
            loggingBuilder.AddSerilog(dispose: true));
    }

    private static void SwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            // داک تجمیعی پیش‌فرض برای سازگاری با مسیر /swagger/v1/swagger.json
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "All APIs",
                Version = "v1"
            });
            // Security JWT
            var securityScheme = new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });

            // Swagger docs برای هر API group
            var apiGroups = new[]
            {
                "Identity",
                "Administrator",
                "Managers",
                "Users",
                "MiniApp"
            };

            foreach (var group in apiGroups)
            {
                options.SwaggerDoc(group, new OpenApiInfo
                {
                    Title = $"API {group}",
                    Version = "v1"
                });
            }

            // گروه‌بندی بر اساس ApiExplorerSettings(GroupName)
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                // اگر داک v1 باشد، همه endpointها را نمایش بده
                if (string.Equals(docName, "v1", StringComparison.OrdinalIgnoreCase))
                    return true;

                var groupName = apiDesc.GroupName;
                if (!string.IsNullOrEmpty(groupName))
                    return string.Equals(groupName, docName, StringComparison.OrdinalIgnoreCase);

                // کنترلرهایی که GroupName ندارند را در گروه "Users" نمایش بده
                return string.Equals(docName, "Users", StringComparison.OrdinalIgnoreCase);
            });
        });
    }

    private static void JwtAuthorizeConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["JWT:Key"];
        if (string.IsNullOrEmpty(jwtKey))
            throw new ArgumentException("JWT Key is not configured");

        var Key = Encoding.UTF8.GetBytes(jwtKey);

        // Ensure key is at least 256 bits (32 bytes) for security
        if (Key.Length < 32)
            throw new ArgumentException("JWT key must be at least 256 bits (32 bytes) long for security");

        var tokenValidationParameter = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JWT:Issuer"],
            ValidAudience = configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Key),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            RequireExpirationTime = true,
            RequireSignedTokens = true
        };
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(option =>
        {
            option.SaveToken = true;
            option.TokenValidationParameters = tokenValidationParameter;

            option.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                    // Map custom role claim if necessary
                    var roleClaims = claimsIdentity.FindAll("role").ToList();
                    foreach (var roleClaim in roleClaims)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddSingleton(tokenValidationParameter);
    }

    private static void FluentValidationConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IValidatorProvider, ValidatorProvider>();
        services.AddValidatorsFromAssemblyContaining<RefreshTokensValidator>();
    }

    private static void ConfigurationDependency(this IServiceCollection services)
    {
        services.AddHttpClient();
    }

    private static void RateLimitingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure IP rate limiting
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

        // Configure client rate limiting
        services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));
        services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));

        // Add rate limiting services
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    }

    private static void MediatRDependency(this IServiceCollection services)
    {
        services.AddMediatR(m =>
        {
            // Register Infrastructure layer (current assembly)
            m.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // Register handlers from ApplicationLayer (CQRS handlers live here)
            var applicationAssembly = typeof(ApplicationLayer.Features.Plans.Handler.GetPlansHandler).Assembly;
            m.RegisterServicesFromAssembly(applicationAssembly);

            // Optionally register handlers from Kavan.Api if any exist
            var presentationAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Kavan.Api");
            if (presentationAssembly != null)
            {
                m.RegisterServicesFromAssembly(presentationAssembly);
            }
        })
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    }

    public static void RegisterServicesAutomatically(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register non-generic types
        var nonGenericTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract
            && (t.GetCustomAttribute<InjectAsScopedAttribute>() != null ||
            t.GetCustomAttribute<InjectAsTransientAttribute>() != null ||
            t.GetCustomAttribute<InjectAsSingletonAttribute>() != null)
            && t.GetInterfaces().Any(i => !i.IsGenericType))
            .Select(t => new
            {
                ServiceType = t.GetInterfaces().FirstOrDefault(),
                ImplementationType = t,
                Scoped = t.GetCustomAttribute<InjectAsScopedAttribute>() != null,
                Transient = t.GetCustomAttribute<InjectAsTransientAttribute>() != null,
                Singleton = t.GetCustomAttribute<InjectAsSingletonAttribute>() != null
            }).Where(x => x.ServiceType != null);

        foreach (var type in nonGenericTypes)
        {
            if (type.Scoped)
            {
                services.AddScoped(type.ServiceType, type.ImplementationType);
            }
            else if (type.Transient)
            {
                services.AddTransient(type.ServiceType, type.ImplementationType);
            }
            else if (type.Singleton)
            {
                services.AddSingleton(type.ServiceType, type.ImplementationType);
            }
        }
    }
}