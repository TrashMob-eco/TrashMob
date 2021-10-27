namespace TrashMobMobile.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public class MobEventRestService
    {
        HttpClient client;
        JsonSerializerOptions serializerOptions;

        public List<MobEvent> MobEvents { get; private set; }

        public MobEventRestService()
        {
            client = new HttpClient();
            serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<List<MobEvent>> RefreshMobEventsAsync()
        {
            MobEvents = new List<MobEvent>();

            Uri uri = new Uri(string.Format(Constants.MobEventsDataUrl, string.Empty));
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    MobEvents = JsonSerializer.Deserialize<List<MobEvent>>(content, serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return MobEvents;
        }
    }
}
