using ApplicationLayer.Common.Behaviors;
using ApplicationLayer.Features.Validations;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Mapping.UserAccounts;
using AspNetCoreRateLimit;
using DomainLayer.Common.Attributes;
using FluentValidation;
using InfrastructureLayer.BusinessLogic.Services.Binance;
using InfrastructureLayer.BusinessLogic.Services.Realtime;
using InfrastructureLayer.BusinessLogic.Services.Signals;
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
using StackExchange.Redis;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace InfrastructureLayer;

public static class DependencyInjection
{
    private static readonly string[] configurePolicy = new[]
            {
                "https://tg.kavan.trade",
                "https://core.kavan.trade",
                "https://panel.kavan.trade",
                "http://localhost:3003",
                "http://localhost:5173",
                "http://localhost:8080",
                "http://127.0.0.1:3003",
                "http://127.0.0.1:5173",
                "http://127.0.0.1:8080",
                "https://api.packsi.net",
                "https://packsi.net",
                "https://kavan.packsi.net",
                "https://kavan-core.packsi.net"
            };

    public static IServiceCollection Register(this IServiceCollection services, IConfiguration configuration, bool isWorker = false)
    {
        services.RegisterService(configuration, isWorker);

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserContextService, UserContextService>();

        // Realtime / Redis
        services.AddSingleton<IConnectionMultiplexer>(sp => 
        {
            var config = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis") ?? "localhost:6379");
            config.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(config);
        });
        services.AddSingleton<ICandleBroadcaster, RedisCandleBroadcaster>();

        // Register BackgroundService(s)
        if (isWorker)
        {
            services.AddScoped<BusinessLogic.Services.External.DuneTxCountSyncService>();
            services.AddScoped<BusinessLogic.Services.External.DuneUserCountSyncService>();
            services.AddScoped<BusinessLogic.Services.External.DuneEtfIssuerFlowSyncService>();

            // Disable legacy 1m REST fetcher
            // services.AddHostedService<CandleFetcherBackgroundService>();
            // Enable Binance 1m Kline Ingestion Service (Worker)
            var active = configuration.GetValue<bool>("BinanceWebSocket:ActiveService");
            if (active) { services.AddHostedService<Binance1mKlineIngestionService>(); }
            // services.AddHostedService<BinanceKlineWebSocketHostedService>();
            services.AddHostedService<BusinessLogic.Services.News.NewsSyncBackgroundService>();
            services.AddHostedService<BusinessLogic.Services.External.DuneSyncBackgroundService>();
            services.AddHostedService<BusinessLogic.Services.External.DuneTxCountBackgroundService>();
            services.AddHostedService<BusinessLogic.Services.External.DuneGasPriceBackgroundService>();
            services.AddHostedService<BusinessLogic.Services.External.DuneUserCountBackgroundService>();
            services.AddHostedService<BusinessLogic.Services.External.DuneEtfIssuerFlowBackgroundService>();
            services.AddHostedService<BusinessLogic.Services.Trading.MarketMonitoringService>();
        }

        services.AddHttpContextAccessor();
        services.MediatRDependency();
        services.RegisterServicesAutomatically();
        services.FluentValidationConfiguration();
        services.RateLimitingConfiguration(configuration);
        services.AddHttpClient("OpenRouterClient", client =>
        {
            var baseUrl = configuration["OpenRouter:BaseUrl"] ?? "https://openrouter.ai/api/v1";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        services.AddCors(opt => opt.AddPolicy("AllowSpecificOrigin", builder =>
        {
            var explicitOrigins = configurePolicy;

            builder
                .SetIsOriginAllowed(origin =>
                {
                    try
                    {
                        var uri = new Uri(origin);
                        var host = uri.Host;
                        return explicitOrigins.Contains(origin)
                               || host.EndsWith("packsi.net")
                               || host.EndsWith("kavan.trade")
                               || host == "localhost"
                               || host == "127.0.0.1";
                    }
                    catch
                    {
                        return false;
                    }
                })
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

    private static void RegisterService(this IServiceCollection services, IConfiguration configuration, bool isWorker)
    {
        services.ConfigurationDependency();
        services.Configure<InfrastructureLayer.Configuration.SignalRetentionOptions>(configuration.GetSection("SignalRetention"));
        services.Configure<InfrastructureLayer.Configuration.CryptoPanicOptions>(configuration.GetSection("CryptoPanic"));
        services.Configure<InfrastructureLayer.Configuration.DuneOptions>(configuration.GetSection("Dune"));
        services.PostConfigure<InfrastructureLayer.Configuration.DuneOptions>(opt =>
        {
            if (string.IsNullOrWhiteSpace(opt.ApiKey))
            {
                opt.ApiKey = Environment.GetEnvironmentVariable("DUNE_API_KEY")
                           ?? configuration["DUNE_API_KEY"]
                           ?? configuration["Dune:ApiKey"]
                           ?? configuration["Dune__ApiKey"]
                           ?? string.Empty;
            }
        });
        services.AddMemoryCache();
        if (!isWorker)
        {
            services.SwaggerConfiguration(configuration);
        }
        services.JwtAuthorizeConfiguration(configuration);
        services.SeriLogConfiguration(configuration);
        services.AddAutoMapper(typeof(UserAccountProfile).GetTypeInfo().Assembly);

        // HttpClient for Binance API
        services.AddHttpClient("BinanceClient", client =>
        {
            client.BaseAddress = new Uri(configuration["BinanceApi:BaseUrl"] ?? "https://api.binance.com");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // HttpClient for Coinbase API
        services.AddHttpClient("CoinbaseClient", client =>
        {
            client.BaseAddress = new Uri(configuration["CoinbaseApi:BaseUrl"] ?? "https://api.exchange.coinbase.com");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient("CryptoPanicClient", client =>
        {
            var baseUrl = configuration["CryptoPanic:BaseUrl"] ?? "https://cryptopanic.com/api/developer/v2";
            if (!baseUrl.EndsWith("/")) baseUrl += "/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient("DuneClient", client =>
        {
            var baseUrl = configuration["Dune:BaseUrl"] ?? "https://api.dune.com/api/v1";
            if (!baseUrl.EndsWith("/")) baseUrl += "/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        // Background Service registration is handled in Register() to avoid duplication
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
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
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
                        }
                    },
                    Array.Empty<string>()
                }
            });

            options.SwaggerDoc("Identity", new OpenApiInfo { Title = "Identity APIs", Version = "v1" });
            options.SwaggerDoc("External", new OpenApiInfo { Title = "External APIs", Version = "v1" });
            options.SwaggerDoc("Admin", new OpenApiInfo { Title = "Admin APIs", Version = "v1" });
            options.SwaggerDoc("Trader", new OpenApiInfo { Title = "Trader APIs", Version = "v1" });
            options.SwaggerDoc("Public", new OpenApiInfo { Title = "Public APIs", Version = "v1" });

            options.DocInclusionPredicate((documentName, apiDesc) =>
                string.Equals(apiDesc.GroupName, documentName, StringComparison.OrdinalIgnoreCase));

            options.CustomSchemaIds(type => type.FullName);
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
            m.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            // Register handlers from ApplicationLayer assembly (CQRS Handlers, Commands, Queries)
            var applicationAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "ApplicationLayer");
            if (applicationAssembly != null)
            {
                m.RegisterServicesFromAssembly(applicationAssembly);
            }
            // Register handlers from Presentation Kavan.Api assembly for domain events
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
