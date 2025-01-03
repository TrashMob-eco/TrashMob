﻿namespace TrashMobMobile.Services;

using System.Diagnostics;
using System.Net.Http.Json;
using Newtonsoft.Json;
using TrashMob.Models;
using TrashMobMobile.Authentication;

public class UserRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IUserRestService
{
    protected override string Controller => "users";

    public async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/" + userId;

        using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<User>(responseString);
        }
    }

    public async Task<User> GetUserByEmailAsync(string email, UserContext userContext,
        CancellationToken cancellationToken = default)
    {
        var localHttpClient = new HttpClient
        {
            BaseAddress = new Uri(string.Concat(TrashMobApiAddress, Controller)),
        };

        localHttpClient.DefaultRequestHeaders.Authorization = GetAuthToken(userContext);
        localHttpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain");

        var requestUri = Controller + "/getuserbyemail/" + email;

        using (var response = await localHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<User>(responseString);
        }
    }

    public async Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(user, typeof(User), null, SerializerOptions);

        using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<User>(responseString);
        }
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(user, typeof(User), null, SerializerOptions);

        using (var response = await AuthorizedHttpClient.PutAsync(Controller, content, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<User>(responseString);
        }
    }
}