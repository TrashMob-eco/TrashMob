namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Threading.Tasks;

    [Route("restapi/v2.1/accounts")]
    public class DocusignController : BaseController
    {
        public DocusignController(TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
        }

        [HttpPost("{accountId}/envelopes")]
        public async Task<IActionResult> SendEnvelope(string accountId, [FromBody] JsonElement body)
        {
            var incomingToken = Request.Headers.Authorization[0];
            var docusignHeader = Request.Headers["X-DocuSign-SDK"][0];
            var docusignApiRoot = "https://demo.docusign.net";
            var path = Request.Path;

            var httpRequestMessage = new HttpRequestMessage();

            httpRequestMessage.Headers.Add("Accept", "application/json, text/plain");
            httpRequestMessage.Headers.Add("Authorization", incomingToken);
            httpRequestMessage.Headers.Add("X-DocuSign-SDK", docusignHeader);
            httpRequestMessage.Method = HttpMethod.Post;

            httpRequestMessage.RequestUri = new Uri(docusignApiRoot + path);
            var jsonString = JsonObject.Create(body).ToJsonString();
            httpRequestMessage.Content = new StringContent(jsonString);

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

            return Ok(response);
        }
    }
}
