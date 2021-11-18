namespace TrashMobMobile
{
    using System;

    public static class Constants
    {
        public const string PrivacyPolicyVersion = "0.3";
        public static DateTimeOffset PrivacyPolicyDate = new DateTimeOffset(2021, 5, 14, 0, 0, 0, TimeSpan.Zero);
        public const string TermsOfServiceVersion = "0.3";
        public static DateTimeOffset TermsOfServiceDate = new DateTimeOffset(2021, 5, 14, 0, 0, 0, TimeSpan.Zero);
        public static string ApiEndpoint = "https://as-tm-dev-westus2.azurewebsites.net/api/";
    }
}
