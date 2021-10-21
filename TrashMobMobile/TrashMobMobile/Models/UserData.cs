namespace TrashMobMobile.Models
{
    using System;

    public class UserData
    {
        public string Id { get; set; }

        public string NameIdentifier { get; set; }

        public string UserName { get; set; }

        public string SourceSystemUserName { get; set; }

        public string GivenName { get; set; }

        public string SurName { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public string Email { get; set; }

        public DateTimeOffset DateAgreedToPrivacyPolicy { get; set; }

        public string PrivacyPolicyVersion { get; set; }

        public DateTimeOffset DateAgreedToTermsOfService { get; set; }

        public string TermsOfServiceVersion { get; set; }

        public DateTimeOffset MemberSince { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool PrefersMetric { get; set; }

        public bool IsOptedOutOfAllEmails { get; set; }

        public int TravelLimitForLocalEvents { get; set; }

        public bool IsSiteAdmin { get; set; }
    }
}
