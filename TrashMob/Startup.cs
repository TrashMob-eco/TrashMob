namespace TrashMob
{
    using System;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Text.Json.Serialization;
    using Azure.Identity;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Identity.Web;
    using Microsoft.OpenApi.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;

    public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; } = configuration;
        private IWebHostEnvironment CurrentEnvironment { get; } = webHostEnvironment;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // The following line enables Application Insights telemetry collection.
            services.AddApplicationInsightsTelemetry();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(options =>
                    {
                        Configuration.Bind("AzureAdB2C", options);

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
                    options => { Configuration.Bind("AzureAdB2C", options); });

            services.AddAuthorizationBuilder()
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
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "client-app/build"; });

            services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            services.AddDbContext<MobDbContext>(c => c.UseLazyLoadingProxies());

            // Security 
            services.AddScoped<IAuthorizationHandler, UserOwnsEntityAuthHandler>();
            services.AddScoped<IAuthorizationHandler, UserIsValidUserAuthHandler>();
            services.AddScoped<IAuthorizationHandler, UserIsAdminAuthHandler>();
            services.AddScoped<IAuthorizationHandler, UserOwnsEntityOrIsAdminAuthHandler>();
            services.AddScoped<IAuthorizationHandler, UserIsPartnerUserOrIsAdminAuthHandler>();

            ServiceBuilder.AddManagers(services);
            ServiceBuilder.AddRepositories(services);

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000/", "https://localhost:3000/")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            if (CurrentEnvironment.IsDevelopment())
            {
                services.AddScoped<IKeyVaultManager, LocalKeyVaultManager>();
                services.AddScoped<IDocusignAuthenticator, DocusignStringAuthenticator>();
                services.AddAzureClients(azureClientFactoryBuilder =>
                {
                    azureClientFactoryBuilder.UseCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions
                    {
                        VisualStudioTenantId = Configuration.GetValue<string>("TrashMobBackendTenantId"),
                    }));
                    azureClientFactoryBuilder.AddBlobServiceClient(Configuration.GetValue<Uri>("StorageAccountUri"));
                });
            }
            else
            {
                services.AddAzureClients(azureClientFactoryBuilder =>
                {
                    azureClientFactoryBuilder.UseCredential(new DefaultAzureCredential());

                    azureClientFactoryBuilder.AddSecretClient(Configuration.GetValue<Uri>("VaultUri"));
                    azureClientFactoryBuilder.AddBlobServiceClient(Configuration.GetValue<Uri>("StorageAccountUri"));
                });

                services.AddScoped<IKeyVaultManager, KeyVaultManager>();
                services.AddScoped<IDocusignAuthenticator, DocusignStringAuthenticator>();
            }

            services.AddControllers();
            services.AddSwaggerGen(options =>
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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

            app.UseCors(MyAllowSpecificOrigins);

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                {
                    spa.Options.SourcePath = "client-app";
                    spa.UseReactDevelopmentServer("start");
                }
            });
        }
    }
}