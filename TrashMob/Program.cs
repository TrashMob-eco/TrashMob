namespace TrashMob
{
    using System;
    using System.Text;
    using System.Text.Json.Nodes;
    using System.Text.Json.Serialization;
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared;
    using System.Text.Json;
    using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Identity.Web;

    public class Program
    {
        private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json", true, true)
                                 .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true); // optional extra provider     

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

            builder.Services.AddControllers()
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.SetIsOriginAllowed(x => true)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddScoped<IKeyVaultManager, LocalKeyVaultManager>();
                builder.Services.AddScoped<IDocusignAuthenticator, DocusignStringAuthenticator>();
                builder.Services.AddAzureClients(azureClientFactoryBuilder =>
                {
                    azureClientFactoryBuilder.UseCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions
                    {
                        VisualStudioTenantId = builder.Configuration.GetValue<string>("TrashMobBackendTenantId"),
                    }));
                    azureClientFactoryBuilder.AddBlobServiceClient(builder.Configuration.GetValue<Uri>("StorageAccountUri"));
                });
            }
            else
            {
                builder.Services.AddAzureClients(azureClientFactoryBuilder =>
                {
                    azureClientFactoryBuilder.UseCredential(new DefaultAzureCredential());

                    azureClientFactoryBuilder.AddSecretClient(builder.Configuration.GetValue<Uri>("VaultUri"));
                    azureClientFactoryBuilder.AddBlobServiceClient(builder.Configuration.GetValue<Uri>("StorageAccountUri"));
                });

                builder.Services.AddScoped<IKeyVaultManager, KeyVaultManager>();
                builder.Services.AddScoped<IDocusignAuthenticator, DocusignStringAuthenticator>();
            }

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
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "trashmobapi v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSpa(spa =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    spa.Options.SourcePath = "client-app";
                    spa.UseReactDevelopmentServer("start");
                }
            });

            app.Run();
        }
    }
}