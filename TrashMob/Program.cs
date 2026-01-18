namespace TrashMob;

using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;
using NetTopologySuite.IO.Converters;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TrashMob.Security;
using TrashMob.Shared;
using TrashMob.Shared.Managers;
using TrashMob.Shared.Managers.Interfaces;
using TrashMob.Shared.Persistence;

public class Program
{
    private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.json", true, true)
                             .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true); // optional extra provider     

        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddJsonFile("appsettings.Development.json", true, true);
        }

        if (!builder.Environment.IsDevelopment())
        {
            var secretClient = new SecretClient(new Uri(builder.Configuration.GetValue<string>("VaultUri")),
                new DefaultAzureCredential());
            builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
        }

        builder.Logging.AddApplicationInsights();
        builder.Services.AddApplicationInsightsTelemetry();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options);

                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.ValidateLifetime = true;
                options.TokenValidationParameters.ValidateAudience = false;

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        if (context.AuthenticateFailure != null)
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";

                            var error = new
                            {
                                errors = new JsonArray
                                    {
                                        new
                                        {
                                            message = "Invalid access token.",
                                        },
                                    },
                            };

                            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(error));
                            await context.Response.Body.WriteAsync(bytes);
                        }
                    },
                };
            },
                options => { builder.Configuration.Bind("AzureAdB2C", options); });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicyConstants.ValidUser,
                policy => policy.AddRequirements(new UserIsValidUserRequirement()))
            .AddPolicy(AuthorizationPolicyConstants.UserOwnsEntity,
                policy => policy.AddRequirements(new UserOwnsEntityRequirement()))
            .AddPolicy(AuthorizationPolicyConstants.UserOwnsEntityOrIsAdmin,
                policy => policy.AddRequirements(new UserOwnsEntityOrIsAdminRequirement()))
            .AddPolicy(AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin,
                policy => policy.AddRequirements(new UserIsPartnerUserOrIsAdminRequirement()))
            .AddPolicy(AuthorizationPolicyConstants.UserIsAdmin,
                policy => policy.AddRequirements(new UserIsAdminRequirement()));

        // In production, the React files will be served from this directory
        builder.Services.AddSpaStaticFiles(configuration => { configuration.RootPath = "client-app/build"; });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(MyAllowSpecificOrigins,
                policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "https://localhost:3000", "https://localhost:44332")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        builder.Services.AddControllers()
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                x.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
                x.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
            });

        builder.Services.AddDbContext<MobDbContext>(c => c.UseLazyLoadingProxies());

        // Security 
        builder.Services.AddScoped<IAuthorizationHandler, UserOwnsEntityAuthHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, UserIsValidUserAuthHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, UserIsAdminAuthHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, UserOwnsEntityOrIsAdminAuthHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, UserIsPartnerUserOrIsAdminAuthHandler>();

        builder.Services.AddManagers();
        builder.Services.AddRepositories();

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        var blobStorageUrl = builder.Configuration.GetValue<Uri>("StorageAccountUri");

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddScoped<IKeyVaultManager, LocalKeyVaultManager>();
            builder.Services.AddAzureClients(azureClientFactoryBuilder =>
            {
                azureClientFactoryBuilder.UseCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    VisualStudioTenantId = builder.Configuration.GetValue<string>("TrashMobBackendTenantId"),
                }));
                azureClientFactoryBuilder.AddBlobServiceClient(blobStorageUrl);
            });
        }
        else
        {
            builder.Services.AddAzureClients(azureClientFactoryBuilder =>
            {
                azureClientFactoryBuilder.UseCredential(new DefaultAzureCredential());

                azureClientFactoryBuilder.AddSecretClient(builder.Configuration.GetValue<Uri>("VaultUri"));
                azureClientFactoryBuilder.AddBlobServiceClient(blobStorageUrl);
            });

            builder.Services.AddScoped<IKeyVaultManager, KeyVaultManager>();
        }

        // builder.Services.AddScoped(serviceProvider => new BlobServiceClient(blobStorageUrl));

        builder.Services.AddHealthChecks()
            .AddSqlServer(
                builder.Configuration["TMDBServerConnectionString"] ?? string.Empty,
                name: "database",
                tags: ["db", "sql", "sqlserver"]);

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "trashmobapi", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });
            
            // Ensure documentation can be read by Swagger 
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic);

            foreach (var assembly in assemblies)
            {
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }
        });

        var app = builder.Build();

        var enableSwagger = builder.Environment.IsDevelopment() ||
                            builder.Configuration.GetValue<bool>("EnableSwagger");

        if (builder.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseForwardedHeaders();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseForwardedHeaders();
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        if (enableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "trashmobapi v1"));
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSpaStaticFiles();

        app.UseRouting();

        app.UseCors(MyAllowSpecificOrigins);

        app.UseAuthentication();
        app.UseAuthorization();

#pragma warning disable ASP0014
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers()
                .RequireCors(MyAllowSpecificOrigins);

            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });
#pragma warning restore ASP0014

        app.UseSpa(spa =>
        {
            if (builder.Environment.IsDevelopment())
            {
                spa.Options.SourcePath = "client-app";
                spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                spa.UseReactDevelopmentServer("start");
            }
        });

        app.Run();
    }
}