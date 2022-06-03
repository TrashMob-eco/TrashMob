
namespace TrashMob.Shared.Persistence
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using static DocuSign.eSign.Client.Auth.OAuth;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public class DocusignManager : IDocusignManager
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<DocusignManager> logger;
        private string clientId;
        private string impersonatedUserId;
        private string authServer;
        private string privateKey;
        private string accountId;
        private string basePath;
        private string redirectHome;

        public DocusignManager(IConfiguration configuration, ILogger<DocusignManager> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            clientId = configuration["DocusignClientId"];
            impersonatedUserId = configuration["DocusignImpersonatedUserId"]; // joe@trashmob.eco
            authServer = configuration["DocusignAuthServer"];
            privateKey = configuration["DocusignPrivateKey"];
            accountId = configuration["DocusignAccountId"];
            basePath = configuration["DocusignBasePath"];
            redirectHome = configuration["DocusignRedirectHome"];
            privateKey = privateKey.Replace("-----BEGIN RSA PRIVATE KEY----- ", "-----BEGIN RSA PRIVATE KEY-----\n");
            privateKey = privateKey.Replace(" -----END RSA PRIVATE KEY-----", "\n-----END RSA PRIVATE KEY-----\n");
        }

        public EnvelopeResponse SendEnvelope(EnvelopeRequest envelopeRequest)
        {
            OAuthToken accessToken;

            try
            {
                accessToken = AuthenticateWithJWT(clientId, impersonatedUserId, authServer, privateKey);
            }
            catch (Exception ex)
            {
                // Consent for impersonation must be obtained to use JWT Grant
                if (ex.Message.Contains("consent_required"))
                {
                    // build a URL to provide consent for this Integratio Key and this userId
                    string url = "https://" + authServer + "/oauth/auth?response_type=code^&scope=impersonation%20signature&client_id=" +
                                clientId + "&redirect_uri=" + redirectHome;
                    logger.LogError($"Consent is required - launch browser (URL is {url})");                    
                }
 
                throw;
            }

            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken.access_token);

            string docxDocument = "Docusign\\TrashMob_Volunteer_Waiver_V1.docx";

            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);

            EnvelopeDefinition envelope = MakeEnvelope(envelopeRequest.SignerEmail, envelopeRequest.SignerName, envelopeRequest.SignerClientId, docxDocument);
            EnvelopeSummary result = envelopesApi.CreateEnvelope(accountId, envelope);

            var envelopeId = result.EnvelopeId;

            // Step 3. create the recipient view, the Signing Ceremony
            RecipientViewRequest viewRequest = MakeRecipientViewRequest(envelopeRequest.SignerEmail, envelopeRequest.SignerName, envelopeRequest.ReturnUrl, envelopeRequest.SignerClientId, envelopeRequest.PingUrl);

            // call the CreateRecipientView API
            ViewUrl results1 = envelopesApi.CreateRecipientView(accountId, envelopeId, viewRequest);

            // Step 4. Redirect the user to the Signing Ceremony
            // Don't use an iFrame!
            // State can be stored/recovered using the framework's session or a
            // query parameter on the returnUrl (see the makeRecipientViewRequest method)
            string redirectUrl = results1.Url;

            // returning both the envelopeId as well as the url to be used for embedded signing
            return new EnvelopeResponse
            {
                EnvelopeId = envelopeId,
                RedirectUrl = redirectUrl
            };
        }

        /// <summary>
        /// Uses Json Web Token (JWT) Authentication Method to obtain the necessary information needed to make API calls.
        /// </summary>
        /// <returns>Auth token needed for API calls</returns>
        public static OAuthToken AuthenticateWithJWT(string clientId, string impersonatedUserId, string authServer, string privateKey)
        {
            var apiClient = new ApiClient();
            var scopes = new List<string>
                {
                    "signature",
                    "impersonation",
                };

            var bytes = Encoding.UTF8.GetBytes(privateKey);
            return apiClient.RequestJWTUserToken(clientId, impersonatedUserId, authServer, bytes, 1, scopes);
        }

        private static EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, Guid signerClientId, string docxDocument)
        {
            string docxDocumentBytes = Convert.ToBase64String(File.ReadAllBytes(docxDocument));

            EnvelopeDefinition env = new EnvelopeDefinition
            {
                EmailSubject = "TrashMob.eco Volunteer Waiver"
            };

            Document waiverDoc = new Document
            {
                DocumentBase64 = docxDocumentBytes,
                Name = "Waiver", // can be different from actual file name
                FileExtension = "docx",
                DocumentId = "1"
            };

            env.Documents = new List<Document> { waiverDoc };

            Signer signer = new Signer
            {
                Email = signerEmail,
                Name = signerName,
                RecipientId = "1",
                RoutingOrder = "1",
                ClientUserId = signerClientId.ToString(),
            };

            SignHere signHere = new SignHere
            {
                AnchorString = "/sn1/",
                AnchorUnits = "pixels",
                AnchorYOffset = "0",
                AnchorXOffset = "0",
                ScaleValue = "1.5",
            };

            FullName fullName = new FullName
            {
                AnchorString = "/fn1/",
                AnchorUnits = "pixels",
                AnchorYOffset = "0",
                AnchorXOffset = "0",
                Font = "Tahoma",
                FontSize = "Size16",
            };

            DateSigned dateSigned = new DateSigned
            {
                AnchorString = "/ds1/",
                AnchorUnits = "pixels",
                AnchorYOffset = "0",
                AnchorXOffset = "0",
                Font = "Tahoma",
                FontSize = "Size16",
            };

            Tabs signerTabs = new Tabs
            {
                SignHereTabs = new List<SignHere> { signHere },
                FullNameTabs = new List<FullName> { fullName },
                DateSignedTabs = new List<DateSigned> { dateSigned },
            };

            signer.Tabs = signerTabs;

            // Add the recipients to the envelope object
            Recipients recipients = new Recipients
            {
                Signers = new List<Signer> { signer },
            };

            env.Recipients = recipients;

            env.Status = "sent";
            return env;
        }

        private static RecipientViewRequest MakeRecipientViewRequest(string signerEmail, string signerName, string returnUrl, Guid signerClientId, string pingUrl = null)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // dsPingUrl -- class global
            // signerClientId -- class global
            // dsReturnUrl -- class global

            RecipientViewRequest viewRequest = new RecipientViewRequest();
            // Set the url where you want the recipient to go once they are done signing
            // should typically be a callback route somewhere in your app.
            // The query parameter is included as an example of how
            // to save/recover state information during the redirect to
            // the DocuSign signing ceremony. It's usually better to use
            // the session mechanism of your web framework. Query parameters
            // can be changed/spoofed very easily.
            viewRequest.ReturnUrl = returnUrl;

            // How has your app authenticated the user? In addition to your app's
            // authentication, you can include authenticate steps from DocuSign.
            // Eg, SMS authentication
            viewRequest.AuthenticationMethod = "none";

            // Recipient information must match embedded recipient info
            // we used to create the envelope.
            viewRequest.Email = signerEmail;
            viewRequest.UserName = signerName;
            viewRequest.ClientUserId = signerClientId.ToString();

            // DocuSign recommends that you redirect to DocuSign for the
            // Signing Ceremony. There are multiple ways to save state.
            // To maintain your application's session, use the pingUrl
            // parameter. It causes the DocuSign Signing Ceremony web page
            // (not the DocuSign server) to send pings via AJAX to your
            // app,
            // NOTE: The pings will only be sent if the pingUrl is an https address
            if (pingUrl != null)
            {
                viewRequest.PingFrequency = "600"; // seconds
                viewRequest.PingUrl = pingUrl; // optional setting
            }

            return viewRequest;
        }

        public async Task<string> GetEnvelopeStatus(string envelopeId)
        {
            OAuthToken accessToken;

            try
            {
                accessToken = AuthenticateWithJWT(clientId, impersonatedUserId, authServer, privateKey);
            }
            catch (Exception ex)
            {
                // Consent for impersonation must be obtained to use JWT Grant
                if (ex.Message.Contains("consent_required"))
                {
                    // build a URL to provide consent for this Integratio Key and this userId
                    string url = "https://" + authServer + "/oauth/auth?response_type=code^&scope=impersonation%20signature&client_id=" +
                                clientId + "&redirect_uri=" + redirectHome;
                    logger.LogError($"Consent is required - launch browser (URL is {url})");
                }

                throw;
            }

            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken.access_token);

            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);

            var envelope = await envelopesApi.GetEnvelopeAsync(accountId, envelopeId);

            return envelope.Status;
        }
    }
}
