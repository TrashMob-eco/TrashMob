namespace TrashMobMobile.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public class MobEventRestService : RestServiceBase, IMobEventRestService
    {
        private readonly Uri EventsApi = new Uri(TrashMobServiceUrlBase + "events");

        public List<MobEvent> MobEvents { get; private set; }

        public async Task<List<MobEvent>> RefreshMobEventsAsync()
        {
            MobEvents = new List<MobEvent>();

            try
            {
                HttpResponseMessage response = await Client.GetAsync(EventsApi);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    MobEvents = JsonSerializer.Deserialize<List<MobEvent>>(content, SerializerOptions);
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
