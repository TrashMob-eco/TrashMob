namespace TrashMob.Shared.Managers.Interfaces
{
    using static DocuSign.eSign.Client.Auth.OAuth;

    public interface IDocusignAuthenticator
    {
        OAuthToken AuthenticateWithJWT(string clientId, string impersonatedUserId, string authServer, out string baseUri);
    }
}
