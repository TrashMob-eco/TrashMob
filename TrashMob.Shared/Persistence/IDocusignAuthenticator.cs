namespace TrashMob.Shared.Persistence
{
    using static DocuSign.eSign.Client.Auth.OAuth;

    public interface IDocusignAuthenticator
    {
        OAuthToken AuthenticateWithJWT(string clientId, string impersonatedUserId, string authServer);
    }
}
