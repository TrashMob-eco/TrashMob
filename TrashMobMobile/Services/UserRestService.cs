namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using Newtonsoft.Json;
using TrashMob.Models;
using TrashMob.Models.Extensions.V2;
using TrashMob.Models.Poco.V2;

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

            return JsonConvert.DeserializeObject<UserDto>(responseString)!.ToEntity();
        }
    }

    public async Task<User> GetUserByEmailAsync(string email,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/getuserbyemail/" + email;

        using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<UserDto>(responseString)!.ToEntity();
        }
    }

    public async Task<User?> GetUserByObjectIdAsync(string objectId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/getbyobjectid/" + objectId;

        using var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode) return null;

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        var dto = JsonConvert.DeserializeObject<UserDto>(responseString);
        return dto?.ToEntity();
    }

    public async Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var writeDto = user.ToWriteDto();
        var content = JsonContent.Create(writeDto, typeof(UserWriteDto), null, SerializerOptions);

        using (var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<UserDto>(responseString)!.ToEntity();
        }
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var writeDto = user.ToWriteDto();
        var content = JsonContent.Create(writeDto, typeof(UserWriteDto), null, SerializerOptions);

        using (var response = await AuthorizedHttpClient.PutAsync(Controller, content, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<UserDto>(responseString)!.ToEntity();
        }
    }
}
