namespace TrashMobMobile.Data
{
    using System.Diagnostics;
    using System.Net.Http.Json;
    using Microsoft.Maui.Devices.Sensors;
    using Newtonsoft.Json;
    using TrashMob.Models;
    using TrashMobMobile.Models;

    public class LitterReportRestService : RestServiceBase, ILitterReportRestService
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
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<LitterReport> AddLitterReportAsync(LitterReport litterReport,
            CancellationToken cancellationToken = default)
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<LitterReport>> GetAllLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            using (var response = await AnonymousHttpClient.GetAsync(Controller, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (string.IsNullOrEmpty(content))
                {
                    return [];
                }

                return JsonConvert.DeserializeObject<List<LitterReport>>(content);
            }
        }

        public async Task<IEnumerable<LitterReport>> GetAssignedLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/assigned";

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
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

        public async Task<IEnumerable<LitterReport>> GetNewLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/new";

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
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

        public async Task<IEnumerable<LitterReport>> GetCleanedLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/cleaned";

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
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

        public async Task<IEnumerable<LitterReport>> GetNotCancelledLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/notcancelled";

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
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

        public async Task<IEnumerable<LitterReport>> GetCancelledLitterReportsAsync(
            CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/cancelled";

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
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
            try
            {
                var requestUri = string.Concat(Controller, $"/{litterReportId}");

                using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task AddLitterImageAsync(Guid litterImageId, string localFileName,
            CancellationToken cancellationToken = default)
        {
            try
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
                        { new StringContent(ImageUploadType.LitterImage), "imageType" }
                    };

                    request.Content = content;

                    using (var response = await AuthorizedHttpClient.SendAsync(request, cancellationToken))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<string> GetLitterImageAsync(Guid litterImageId, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = Controller + "/image/" + litterImageId;

                using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    return content.TrimStart('"').TrimEnd('"');
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task AddLitterImageAsync(Guid litterReportId, Guid litterImageId, string localFileName,
            CancellationToken cancellationToken = default)
        {
            try
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
                        { new StringContent(ImageUploadType.Pickup), "imageType" }
                    };

                    request.Content = content;

                    using (var response = await AuthorizedHttpClient.SendAsync(request, cancellationToken))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<TrashMob.Models.Poco.Location>> GetLocationsByTimeRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/locationsbytimerange?startTime=" + startDate + "&endTime=" + endDate;

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
    }
}