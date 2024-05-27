namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using TrashMob.Models;
using TrashMobMobile.Authentication;

public interface IUserService
{
    Task<User> GetUserByEmailAsync(string email);
}

public class UserService : IUserService
{
    private readonly IHttpClientFactory httpClientFactory;
    
    public UserService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var httpClient = httpClientFactory.CreateClient(AuthConstants.AuthenticatedClient);
        var user = await httpClient.GetFromJsonAsync<User>($"users/getuserbyemail/{email}");

        return user;
    }
}