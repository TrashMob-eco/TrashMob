using System.Net.Http.Json;
using TrashMob.Models;
using TrashMobMobileApp.Authentication;

namespace TrashMobMobileApp.Services;

public interface IUserService
{
    Task<User> GetUserByEmailAsync(string email);
}

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    
    public UserService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient(AuthConstants.AUTHENTICATED_CLIENT);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _httpClient.GetFromJsonAsync<User>($"users/getuserbyemail/{email}");

        return user;
    }
}