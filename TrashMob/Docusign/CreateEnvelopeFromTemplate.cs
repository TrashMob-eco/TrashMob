namespace TrashMob.Docusign
{
    using System;
    using System.Collections.Generic;
    using DocuSign.eSign.Api;
    using DocuSign.eSign.Client;
    using DocuSign.eSign.Model;
    using TrashMob.Common;

    public static class CreateEnvelopeFromTemplate
    {
        /// <summary>
        /// Creates a new envelope from an existing template and send it out
        /// </summary>
        /// <param name="signerEmail">Email address for the signer</param>
        /// <param name="signerName">Full name of the signer</param>
        /// <param name="ccEmail">Email address for the cc recipient</param>
        /// <param name="ccName">Name of the cc recipient</param>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="templateId">The templateId for the tempalte to use to create an envelope</param>
        /// <returns>EnvelopeId for the new envelope</returns>
        public static EnvelopeResponse SendEnvelopeFromTemplate(EnvelopeRequest envelopeRequest)
        {
            var apiClient = new ApiClient(envelopeRequest.BasePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + envelopeRequest.AccessToken);
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeDefinition envelope = MakeEnvelope(envelopeRequest.SignerEmail, envelopeRequest.SignerName, envelopeRequest.SignerClientId, envelopeRequest.TemplateId);
            EnvelopeSummary result = envelopesApi.CreateEnvelope(envelopeRequest.AccountId, envelope);

            var envelopeId = result.EnvelopeId;

            // Step 3. create the recipient view, the Signing Ceremony
            RecipientViewRequest viewRequest = MakeRecipientViewRequest(envelopeRequest.SignerEmail, envelopeRequest.SignerName, envelopeRequest.ReturnUrl, envelopeRequest.SignerClientId, envelopeRequest.PingUrl);
            
            // call the CreateRecipientView API
            ViewUrl results1 = envelopesApi.CreateRecipientView(envelopeRequest.AccountId, envelopeId, viewRequest);

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

        private static EnvelopeDefinition MakeEnvelope(string signerEmail, string signerName, Guid signerClientId, string templateId)
        {
            // Data for this method
            // signerEmail 
            // signerName
            // ccEmail
            // ccName
            // templateId

            EnvelopeDefinition env = new EnvelopeDefinition
            {
                TemplateId = templateId
            };

            TemplateRole signer1 = new TemplateRole
            {
                Email = signerEmail,
                Name = signerName,
                ClientUserId = signerClientId.ToString(),
                RoleName = "signer"
            };

            env.TemplateRoles = new List<TemplateRole> { signer1 };
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
            viewRequest.ReturnUrl = returnUrl + "?state=123";

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
    }
}
