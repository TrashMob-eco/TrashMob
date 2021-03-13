namespace FlashTrashMob.Web
{
    using System;
    using Azure.Security.KeyVault.Secrets;
    using Azure.Identity;
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (context.HostingEnvironment.IsProduction())
                    {
                        var builtConfig = config.Build();
                        var secretClient = new SecretClient(new Uri($"https://{builtConfig["KeyVaultName"]}.vault.azure.net/"),
                                                                new DefaultAzureCredential());
                        config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
