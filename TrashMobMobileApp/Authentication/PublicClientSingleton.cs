namespace TrashMobMobileApp.Authentication
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Identity.Client;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using TrashMobMobileApp.Config;
    using TrashMobMobileApp.Data;

    /// <summary>
    /// This is a singleton implementation to wrap the MSALClient and associated classes to support static initialization model for platforms that need this.
    /// </summary>
    public class PublicClientSingleton
    {
        /// <summary>
        /// This is the singleton used by Ux. Since PublicClientWrapper constructor does not have perf or memory issue, it is instantiated directly.
        /// </summary>
        public static PublicClientSingleton Instance { get; private set; } = new PublicClientSingleton();

        /// <summary>
        /// This is the configuration for the application found within the 'appsettings.json' file.
        /// </summary>
        private static IConfiguration AppConfiguration;

        /// <summary>
        /// Gets the instance of MSALClientHelper.
        /// </summary>
        public DownstreamApiHelper DownstreamApiHelper { get; }

        /// <summary>
        /// Gets the instance of MSALClientHelper.
        /// </summary>
        public MSALClientHelper MSALClientHelperInstance { get; private set; }

        /// <summary>
        /// This will determine if the Interactive Authentication should be Embedded or System view
        /// </summary>
        public bool UseEmbedded { get; set; } = false;

        //// Custom logger for sample
        //private readonly IdentityLogger _logger = new IdentityLogger();

        /// <summary>
        /// Prevents a default instance of the <see cref="PublicClientSingleton"/> class from being created. or a private constructor for singleton
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private PublicClientSingleton()
        {
#if DEBUG
            var embeddedConfigfilename = "TrashMobMobileApp.appSettings.Development.json";
#else
        var embeddedConfigfilename = "TrashMobMobileApp.appSettings.json"; 
#endif
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MauiProgram)).Assembly;
            // Load config
            using var stream = assembly.GetManifestResourceStream(embeddedConfigfilename);
            AppConfiguration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            var settings = AppConfiguration.GetSection("Settings").Get<Settings>();
            this.MSALClientHelperInstance = new MSALClientHelper(settings.AzureADB2C);

            DownStreamApiConfig downStreamApiConfig = settings.DownStreamApi;
            DownstreamApiHelper = new DownstreamApiHelper(downStreamApiConfig, this.MSALClientHelperInstance);
        }

        /// <summary>
        /// Acquire the token silently
        /// </summary>
        /// <returns>An access token</returns>
        public async Task<string> AcquireTokenSilentAsync()
        {
            // Get accounts by policy
            return await this.AcquireTokenSilentAsync(this.GetScopes()).ConfigureAwait(false);
        }

        /// <summary>
        /// Acquire the token silently
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns>An access token</returns>
        public async Task<string> AcquireTokenSilentAsync(string[] scopes)
        {
            return await this.MSALClientHelperInstance.SignInUserAndAcquireAccessToken(scopes).ConfigureAwait(false);
        }

        /// <summary>
        /// Perform the interactive acquisition of the token for the given scope
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns></returns>
        internal async Task<AuthenticationResult> AcquireTokenInteractiveAsync(string[] scopes)
        {
            this.MSALClientHelperInstance.UseEmbedded = this.UseEmbedded;
            return await this.MSALClientHelperInstance.SignInUserInteractivelyAsync(scopes).ConfigureAwait(false);
        }

        /// <summary>
        /// It will sign out the user.
        /// </summary>
        /// <returns></returns>
        internal async Task SignOutAsync()
        {
            await this.MSALClientHelperInstance.SignOutUserAsync().ConfigureAwait(false);
        }

        internal async Task DeleteAccountAsync()
        {
            await this.MSALClientHelperInstance.DeleteUserAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets scopes for the application
        /// </summary>
        /// <returns>An array of all scopes</returns>
        internal string[] GetScopes()
        {
            return this.DownstreamApiHelper.DownstreamApiConfig.ScopesArray;
        }
    }
}