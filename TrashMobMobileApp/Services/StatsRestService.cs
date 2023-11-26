namespace TrashMobMobileApp.Data
{
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using TrashMob.Models.Poco;
    using TrashMobMobileApp.Config;

    public class StatsRestService : RestServiceBase, IStatsRestService
    {
        protected override string Controller => "stats";

        public StatsRestService(IOptions<Settings> settings) 
            : base(settings)
        {
        }

        public async Task<Stats> GetStatsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using (var response = await AnonymousHttpClient.GetAsync(Controller, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                    return JsonConvert.DeserializeObject<Stats>(responseString);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }
    }
}
