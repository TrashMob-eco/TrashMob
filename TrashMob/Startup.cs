namespace TrashMob
{
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Identity.Web;
    using Microsoft.OpenApi.Models;
    using System;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;
    
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

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "client-app/build";
            });

            services.AddDbContext<MobDbContext>();

            // Non-patterned
            services.AddScoped<IDocusignManager, DocusignManager>();

            // Migrated Repositories
            services.AddScoped<IRepository<CommunityAttachment>, Repository<CommunityAttachment>>();
            services.AddScoped<IRepository<CommunityContact>, Repository<CommunityContact>>();
            services.AddScoped<ILookupRepository<CommunityContactType>, LookupRepository<CommunityContactType>>();
            services.AddScoped<IRepository<Community>, Repository<Community>>();
            services.AddScoped<IRepository<CommunityNote>, Repository<CommunityNote>>();
            services.AddScoped<IRepository<CommunityPartner>, Repository<CommunityPartner>>();
            services.AddScoped<IRepository<CommunityRequest>, Repository<CommunityRequest>>();
            services.AddScoped<IRepository<CommunitySocialMediaAccount>, Repository<CommunitySocialMediaAccount>>();
            services.AddScoped<ILookupRepository<CommunityStatus>, LookupRepository<CommunityStatus>>();
            services.AddScoped<IRepository<CommunityUser>, Repository<CommunityUser>>();
            services.AddScoped<IRepository<ContactRequest>, Repository<ContactRequest>>();
            services.AddScoped<IRepository<SocialMediaAccount>, Repository<SocialMediaAccount>>();
            services.AddScoped<ILookupRepository<SocialMediaAccountType>, LookupRepository<SocialMediaAccountType>>();

            // Migrated Managers
            services.AddScoped<IExtendedManager<CommunityAttachment>, CommunityAttachmentManager>();
            services.AddScoped<IExtendedManager<CommunityContact>, CommunityContactManager>();
            services.AddScoped<ILookupManager<CommunityContactType>, CommunityContactTypeManager>();
            services.AddScoped<IExtendedManager<Community>, CommunityManager>();
            services.AddScoped<IExtendedManager<CommunityNote>, CommunityNoteManager>();
            services.AddScoped<IExtendedManager<CommunityPartner>, CommunityPartnerManager>();
            services.AddScoped<IExtendedManager<CommunityRequest>, CommunityRequestManager>();
            services.AddScoped<IExtendedManager<CommunitySocialMediaAccount>, CommunitySocialMediaAccountManager>();
            services.AddScoped<ILookupManager<CommunityStatus>, CommunityStatusManager>();
            services.AddScoped<IExtendedManager<CommunityUser>, CommunityUserManager>();
            services.AddScoped<IManager<ContactRequest>, ContactRequestManager>();
            services.AddScoped<IExtendedManager<SocialMediaAccount>, SocialMediaAccountManager>();
            services.AddScoped<ILookupManager<SocialMediaAccountType>, SocialMediaAccountTypeManager>();

            // Not Migrated Repositories and Managers
            services.AddScoped<IEmailManager, EmailManager>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IEventAttendeeRepository, EventAttendeeRepository>();
            services.AddScoped<IEventMediaRepository, EventMediaRepository>();
            services.AddScoped<IEventPartnerRepository, EventPartnerRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventPartnerStatusRepository, EventPartnerStatusRepository>();
            services.AddScoped<IEventStatusRepository, EventStatusRepository>();
            services.AddScoped<IEventSummaryRepository, EventSummaryRepository>();
            services.AddScoped<IEventTypeRepository, EventTypeRepository>();
            services.AddScoped<IMapRepository, MapRepository>();
            services.AddScoped<IMediaTypeRepository, MediaTypeRepository>();
            services.AddScoped<IMessageRequestManager, MessageRequestManager>();
            services.AddScoped<IMessageRequestRepository, MessageRequestRepository>();
            services.AddScoped<INonEventUserNotificationRepository, NonEventUserNotificationRepository>();
            services.AddScoped<INotificationManager, NotificationManager>();
            services.AddScoped<IPartnerLocationRepository, PartnerLocationRepository>();
            services.AddScoped<IPartnerManager, PartnerManager>();
            services.AddScoped<IPartnerRepository, PartnerRepository>();
            services.AddScoped<IPartnerStatusRepository, PartnerStatusRepository>();
            services.AddScoped<IPartnerRequestRepository, PartnerRequestRepository>();
            services.AddScoped<IPartnerRequestStatusRepository, PartnerRequestStatusRepository>();
            services.AddScoped<IPartnerUserRepository, PartnerUserRepository>();
            services.AddScoped<ISecretRepository, SecretRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();

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
