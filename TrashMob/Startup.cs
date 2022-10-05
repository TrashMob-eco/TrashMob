namespace TrashMob
{
    using Azure.Identity;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Identity.Web;
    using Microsoft.OpenApi.Models;
    using System;
    using System.Text.Json.Serialization;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Events;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.Partners;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Persistence.Interfaces;

    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            CurrentEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment CurrentEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // The following line enables Application Insights telemetry collection.
            services.AddApplicationInsightsTelemetry();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(options => {
                    Configuration.Bind("AzureAdB2C", options);

                    options.TokenValidationParameters.NameClaimType = "name";
                },

            options => { Configuration.Bind("AzureAdB2C", options); });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ValidUser", policy => policy.AddRequirements(new UserIsValidUserRequirement()));
                options.AddPolicy("UserOwnsEntity", policy => policy.AddRequirements(new UserOwnsEntityRequirement()));
                options.AddPolicy("UserOwnsEntityOrIsAdmin", policy => policy.AddRequirements(new UserOwnsEntityOrIsAdminRequirement()));
                options.AddPolicy("UserIsPartnerUserOrIsAdmin", policy => policy.AddRequirements(new UserIsPartnerUserOrIsAdminRequirement()));
                options.AddPolicy("UserIsAdmin", policy => policy.AddRequirements(new UserIsAdminRequirement()));
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "client-app/build";
            });

            services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddDbContext<MobDbContext>(c => c.UseLazyLoadingProxies());

            // Security 
            services.AddScoped<IAuthorizationHandler, UserOwnsEntityAuthHandler>();
            services.AddScoped<IAuthorizationHandler, UserIsValidUserAuthHandler>();
            services.AddScoped<IAuthorizationHandler, UserIsAdminAuthHandler>();
            services.AddScoped<IAuthorizationHandler, UserOwnsEntityOrIsAdminAuthHandler>();
            services.AddScoped<IAuthorizationHandler, UserIsPartnerUserOrIsAdminAuthHandler>();

            // Non-patterned
            services.AddScoped<IDocusignManager, DocusignManager>();
            services.AddScoped<IEmailManager, EmailManager>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IMapManager, MapManager>();
            services.AddScoped<ISecretRepository, SecretRepository>();
            services.AddScoped<INotificationManager, NotificationManager>();

            // Migrated Repositories
            services.AddScoped<IKeyedRepository<ContactRequest>, KeyedRepository<ContactRequest>>();
            services.AddScoped<IKeyedRepository<Event>, KeyedRepository<Event>>();
            services.AddScoped<IBaseRepository<EventAttendee>, BaseRepository<EventAttendee>>();
            services.AddScoped<IBaseRepository<EventPartner>, BaseRepository<EventPartner>>();
            services.AddScoped<ILookupRepository<EventPartnerStatus>, LookupRepository<EventPartnerStatus>>();
            services.AddScoped<ILookupRepository<EventStatus>, LookupRepository<EventStatus>>();
            services.AddScoped<IBaseRepository<EventSummary>, BaseRepository<EventSummary>>();
            services.AddScoped<ILookupRepository<EventType>, LookupRepository<EventType>>();
            services.AddScoped<IKeyedRepository<MessageRequest>, KeyedRepository<MessageRequest>>();
            services.AddScoped<IKeyedRepository<NonEventUserNotification>, KeyedRepository<NonEventUserNotification>>();
            services.AddScoped<IKeyedRepository<Partner>, KeyedRepository<Partner>>();
            services.AddScoped<IKeyedRepository<PartnerContact>, KeyedRepository<PartnerContact>>();
            services.AddScoped<IKeyedRepository<PartnerDocument>, KeyedRepository<PartnerDocument>>();
            services.AddScoped<IBaseRepository<PartnerLocation>, BaseRepository<PartnerLocation>>();
            services.AddScoped<IKeyedRepository<PartnerRequest>, KeyedRepository<PartnerRequest>>();
            services.AddScoped<ILookupRepository<PartnerRequestStatus>, LookupRepository<PartnerRequestStatus>>();
            services.AddScoped<IKeyedRepository<PartnerSocialMediaAccount>, KeyedRepository<PartnerSocialMediaAccount>>();
            services.AddScoped<ILookupRepository<PartnerStatus>, LookupRepository<PartnerStatus>>();
            services.AddScoped<ILookupRepository<PartnerType>, LookupRepository<PartnerType>>();
            services.AddScoped<IBaseRepository<PartnerUser>, BaseRepository<PartnerUser>>();
            services.AddScoped<ILookupRepository<ServiceType>, LookupRepository<ServiceType>>();
            services.AddScoped<ILookupRepository<SocialMediaAccountType>, LookupRepository<SocialMediaAccountType>>();
            services.AddScoped<IKeyedRepository<User>, KeyedRepository<User>>();
            services.AddScoped<IKeyedRepository<UserNotification>, KeyedRepository<UserNotification>>();

            // Migrated Managers
            services.AddScoped<IKeyedManager<ContactRequest>, ContactRequestManager>();
            services.AddScoped<IBaseManager<EventAttendee>, EventAttendeeManager>();
            services.AddScoped<IKeyedManager<Event>, EventManager>();
            services.AddScoped<ILookupManager<EventPartnerStatus>, EventPartnerStatusManager>();
            services.AddScoped<IBaseManager<EventPartner>, EventPartnerManager>();
            services.AddScoped<ILookupManager<EventStatus>, EventStatusManager>();
            services.AddScoped<IBaseManager<EventSummary>, EventSummaryManager>();
            services.AddScoped<ILookupManager<EventType>, EventTypeManager>();
            services.AddScoped<IKeyedManager<MessageRequest>, MessageRequestManager>();
            services.AddScoped<IKeyedManager<Partner>, PartnerManager>();
            services.AddScoped<IKeyedManager<PartnerDocument>, PartnerDocumentManager>();
            services.AddScoped<IKeyedManager<PartnerContact>, PartnerContactManager>();
            services.AddScoped<IKeyedManager<PartnerLocation>, PartnerLocationManager>();
            services.AddScoped<IKeyedManager<PartnerRequest>, PartnerRequestManager>();
            services.AddScoped<ILookupManager<PartnerRequestStatus>, PartnerRequestStatusManager>();
            services.AddScoped<IKeyedManager<PartnerSocialMediaAccount>, PartnerSocialMediaAccountManager>();
            services.AddScoped<ILookupManager<PartnerStatus>, PartnerStatusManager>();
            services.AddScoped<ILookupManager<PartnerType>, PartnerTypeManager>();
            services.AddScoped<IBaseManager<PartnerUser>, PartnerUserManager>();
            services.AddScoped<ILookupManager<ServiceType>, ServiceTypeManager>();
            services.AddScoped<ILookupManager<SocialMediaAccountType>, SocialMediaAccountTypeManager>();
            services.AddScoped<IKeyedManager<NonEventUserNotification>, NonEventUserNotificationManager>();
            services.AddScoped<IKeyedManager<User>, UserManager>();
            services.AddScoped<IKeyedManager<UserNotification>, UserNotificationManager>();

            // Intentional deviation due to unique methods
            services.AddScoped<IEventAttendeeManager, EventAttendeeManager>();
            services.AddScoped<IEventManager, EventManager>();
            services.AddScoped<IEventPartnerManager, EventPartnerManager>();
            services.AddScoped<IPartnerRequestManager, PartnerRequestManager>();
            services.AddScoped<IUserManager, UserManager>();

            services.AddDatabaseDeveloperPageExceptionFilter();
            
            if (CurrentEnvironment.IsDevelopment())
            {
                services.AddScoped<IKeyVaultManager, LocalKeyVaultManager>();
                services.AddScoped<IDocusignAuthenticator, DocusignStringAuthenticator>();
            }
            else
            {
                services.AddAzureClients(azureClientFactoryBuilder =>
                {
                    azureClientFactoryBuilder.UseCredential(new DefaultAzureCredential());
                    azureClientFactoryBuilder.AddSecretClient(Configuration.GetValue<Uri>("VaultUri"));
                });

                services.AddScoped<IKeyVaultManager, KeyVaultManager>();
                services.AddScoped<IDocusignAuthenticator, DocusignStringAuthenticator>();
            }

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "trashmobapi", Version = "v1" });
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "client-app";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
