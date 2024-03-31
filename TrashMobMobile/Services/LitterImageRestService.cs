namespace TrashMobMobile.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobile.Models;

    public class LitterImageRestService : RestServiceBase, ILitterImageRestService
    {
        protected override string Controller => "litterimage";

        public LitterImageRestService()
        {
        }

        public async Task<IEnumerable<LitterImage>> GetLitterImagesAsync(Guid litterReportId, CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/" + litterReportId;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<IEnumerable<LitterImage>>(content);
            }
        }

        public async Task<LitterImage> GetLitterImageAsync(Guid litterImageId, CancellationToken cancellationToken = default)
        {
            var requestUri = Controller + "/" + litterImageId;

            using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<LitterImage>(content);
            }
        }

        public async Task<LitterImage> UpdateLitterImageAsync(LitterImage litterImage, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(litterImage, typeof(LitterImage), null, SerializerOptions);

                using (var response = await AuthorizedHttpClient.PutAsync(Controller, content, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                }

                return await GetLitterImageAsync(litterImage.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<LitterImage> AddLitterImageAsync(LitterImage litterImage, CancellationToken cancellationToken = default)
        {
            try
            {
                var content = JsonContent.Create(litterImage, typeof(LitterImage), null, SerializerOptions);

                using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                }

                return await GetLitterImageAsync(litterImage.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<string> GetLitterImageFileAsync(Guid litterImageId, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = Controller + "/image/" + litterImageId;

                using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync(cancellationToken);
                    return content.TrimStart('"').TrimEnd('"');
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task AddLitterImageFileAsync(Guid litterReportId, Guid litterImageId, string localFileName, CancellationToken cancellationToken = default)
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
                        { streamContent, "formFile", Path.GetFileName(localFileName)},
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
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public Task DeleteLitterImageAsync(Guid litterImageId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
