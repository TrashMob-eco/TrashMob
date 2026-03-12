namespace TrashMobMobile.Services
{
    using System.Globalization;
    using System.Net.Http.Json;
    using Newtonsoft.Json;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMobMobile.Models;

    public class LitterReportRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), ILitterReportRestService
    {
        protected override string Controller => "litterreports";

        public async Task<LitterReport> GetLitterReportAsync(Guid litterReportId,
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/" + litterReportId;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var dto = JsonConvert.DeserializeObject<LitterReportDto>(content)!;
                return dto.ToEntity();
            }
        }

        public async Task<string> GetLitterImageUrlAsync(Guid litterImageId, ImageSizeEnum imageSize,
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/image/" + litterImageId + "/" + imageSize;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                if (!response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return string.Empty;
                }

                var result = await response.Content.ReadAsStringAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(result))
                {
                    return string.Empty;
                }

                return JsonConvert.DeserializeObject<string>(result) ?? string.Empty;
            }
        }

        public async Task<LitterReport> UpdateLitterReportAsync(LitterReport litterReport,
            CancellationToken cancellationToken = default)
        {
            var dto = litterReport.ToV2Dto();
            var content = JsonContent.Create(dto, typeof(LitterReportDto), null, SerializerOptions);

            using (var response = await AuthorizedHttpClient.PutAsync(Controller, content, cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new HttpRequestException(
                        $"Server returned {(int)response.StatusCode} ({response.StatusCode}): {errorBody}");
                }

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<LitterReportDto>(responseContent);

                if (result != null)
                {
                    // Only upload images that have a local file path (new photos, not existing server URLs)
                    foreach (var litterImage in litterReport.LitterImages.Where(l =>
                                 !string.IsNullOrEmpty(l.AzureBlobURL) &&
                                 !l.AzureBlobURL.StartsWith("http", StringComparison.OrdinalIgnoreCase)))
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
            var dto = litterReport.ToV2Dto();
            var content = JsonContent.Create(dto, typeof(LitterReportDto), null, SerializerOptions);

            using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new HttpRequestException(
                        $"Server returned {(int)response.StatusCode} ({response.StatusCode}): {errorBody}");
                }

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<LitterReportDto>(responseContent);

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

                var paged = JsonConvert.DeserializeObject<PaginatedResponseDto<LitterReportDto>>(returnContent)!;
                var entityItems = paged.Items.Select(d => d.ToEntity()).ToList();
                var pageSize = filter.PageSize.GetValueOrDefault(entityItems.Count);
                return new PaginatedList<LitterReport>(entityItems, paged.TotalPages * pageSize, paged.PageIndex, pageSize);
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

                var dtos = JsonConvert.DeserializeObject<IEnumerable<LitterReportDto>>(content) ?? [];
                return dtos.Select(d => d.ToEntity());
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

                return JsonConvert.DeserializeObject<IEnumerable<TrashMob.Models.Poco.Location>>(content) ?? [];
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
                        if (!response.IsSuccessStatusCode)
                        {
                            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                            throw new HttpRequestException(
                                $"Image upload failed with {(int)response.StatusCode} ({response.StatusCode}): {errorBody}");
                        }
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