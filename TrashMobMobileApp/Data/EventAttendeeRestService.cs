namespace TrashMobMobileApp.Data
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public class EventAttendeeRestService : RestServiceBase, IEventAttendeeRestService
    {
        private readonly string EventAttendeeApi = TrashMobServiceUrlBase + "eventattendee";

        public async Task AddAttendeeAsync(EventAttendee eventAttendee)
        {
            try
            {
                //var userContext = await GetUserContext().ConfigureAwait(false);

                var httpRequestMessage = new HttpRequestMessage();
                //httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);

                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.RequestUri = new Uri(EventAttendeeApi);
                httpRequestMessage.Content = JsonContent.Create(eventAttendee, typeof(EventAttendee), null, SerializerOptions);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return;
        }

        public async Task RemoveAttendeeAsync(EventAttendee eventAttendee)
        {
            try
            {
                //var userContext = await GetUserContext().ConfigureAwait(false);

                var httpRequestMessage = new HttpRequestMessage();
                //httpRequestMessage.Headers.Add("Authorization", "BEARER " + userContext.AccessToken);

                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Delete;
                httpRequestMessage.RequestUri = new Uri(EventAttendeeApi + $"/{eventAttendee.EventId}/{eventAttendee.UserId}");

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return;
        }
    }
}
