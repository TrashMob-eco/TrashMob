namespace TrashMobMobile.Services
{
    using System.Globalization;
    using System.Net.Http.Json;
    using Newtonsoft.Json;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMobMobile.Models;

    public class LitterReportRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), ILitterReportRestService
    {
        protected override string Controller => "litterreport";

        public async Task<LitterReport> GetLitterReportAsync(Guid litterReportId,
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/" + litterReportId;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<LitterReport>(content);
            }
        }

        public async Task<string> GetLitterImageUrlAsync(Guid litterImageId, ImageSizeEnum imageSize,
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/image/" + litterImageId + "/" + imageSize;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<string>(result);
            }
        }

        public async Task<LitterReport> UpdateLitterReportAsync(LitterReport litterReport,
            CancellationToken cancellationToken = default)
        {
            var content = JsonContent.Create(litterReport, typeof(LitterReport), null, SerializerOptions);

            using (var response = await AuthorizedHttpClient.PutAsync(Controller, content, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<LitterReport>(responseContent);

                if (result != null)
                {
                    // Only add images that have not been uploaded yet
                    foreach (var litterImage in litterReport.LitterImages.Where(l =>
                                 l.LastUpdatedByUserId == Guid.Empty))
                    {
                        await AddLitterImageAsync(litterImage.Id, litterImage.AzureBlobURL, cancellationToken);
                    }
                }
            }

            return await GetLitterReportAsync(litterReport.Id, cancellationToken);
        }

        public async Task<LitterReport> AddLitterReportAsync(LitterReport litterReport,
            CancellationToken cancellationToken = default)
        {
            var content = JsonContent.Create(litterReport, typeof(LitterReport), null, SerializerOptions);

            using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
            {
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<LitterReport>(responseContent);

                if (result != null)
                {
                    foreach (var litterImage in litterReport.LitterImages)
                    {
                        await AddLitterImageAsync(litterImage.Id, litterImage.AzureBlobURL, cancellationToken);
                    }
                }
            }

            return await GetLitterReportAsync(litterReport.Id, cancellationToken);
        }

        public async Task<PaginatedList<LitterReport>> GetLitterReportsAsync(LitterReportFilter filter, CancellationToken cancellationToken = default)
        {
            var content = JsonContent.Create(filter, typeof(LitterReportFilter), null, SerializerOptions);
            var requestUri = Controller + "/pagedfilteredlitterreports";

            using (var response = await AnonymousHttpClient.PostAsync(requestUri, content, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var returnContent = await response.Content.ReadAsStringAsync(cancellationToken);

                return JsonConvert.DeserializeObject<PaginatedList<LitterReport>>(returnContent);
            }
        }

        public async Task<IEnumerable<LitterReport>> GetUserLitterReportsAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/userlitterreports/" + userId;

            using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (string.IsNullOrEmpty(content))
                {
                    return [];
                }

                return JsonConvert.DeserializeObject<IEnumerable<LitterReport>>(content);
            }
        }

        public async Task DeleteLitterReportAsync(Guid litterReportId, CancellationToken cancellationToken = default)
        {
                var requestUri = string.Concat(Controller, $"/{litterReportId}");

                using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                }
        }

        public async Task<IEnumerable<TrashMob.Models.Poco.Location>> GetLocationsByTimeRangeAsync(
            DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
        {
            var startDateTime = startDate.ToString();
            var endDateTime = endDate.ToString();

            // Convert the DateTime string into the correct Format if the Culture is not Invariant
            if (CultureInfo.CurrentCulture != CultureInfo.InvariantCulture)
            {
                startDateTime = startDate.ToString(@"yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US"));
                endDateTime = endDate.ToString(@"yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US"));
            }

            var requestUri = Controller + "/locationsbytimerange?startTime=" + startDateTime + "&endTime=" + endDateTime;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (string.IsNullOrEmpty(content))
                {
                    return [];
                }

                return JsonConvert.DeserializeObject<IEnumerable<TrashMob.Models.Poco.Location>>(content);
            }
        }

        public async Task AddLitterImageAsync(Guid litterImageId, string localFileName,
            CancellationToken cancellationToken = default)
        {
                var requestUri = Controller + "/image/" + litterImageId;

                using (var stream = File.OpenRead(localFileName))
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.Add("Content-Type", "image/jpeg");

                    var content = new MultipartFormDataContent
                    {
                        { streamContent, "formFile", Path.GetFileName(localFileName) },
                        { new StringContent(litterImageId.ToString()), "parentId" },
                        { new StringContent(ImageUploadType.LitterImage), "imageType" },
                    };

                    request.Content = content;

                    using (var response = await AuthorizedHttpClient.SendAsync(request, cancellationToken))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
        }

        public async Task<string> GetLitterImageAsync(Guid litterImageId, CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/image/" + litterImageId;

            using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return content.TrimStart('"').TrimEnd('"');
            }
        }

        public async Task AddLitterImageAsync(Guid litterReportId, Guid litterImageId, string localFileName,
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/image/" + litterReportId;

            using (var stream = File.OpenRead(localFileName))
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

                var streamContent = new StreamContent(stream);
                streamContent.Headers.Add("Content-Type", "image/jpeg");

                var content = new MultipartFormDataContent
                    {
                        { streamContent, "formFile", Path.GetFileName(localFileName) },
                        { new StringContent(litterImageId.ToString()), "parentId" },
                        { new StringContent(ImageUploadType.Pickup), "imageType" },
                    };

                request.Content = content;

                using (var response = await AuthorizedHttpClient.SendAsync(request, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }
}