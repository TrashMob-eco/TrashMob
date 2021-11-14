namespace TrashMobMobile.Models
{
    using System;

    public class User
    {
        public string Id { get; set; }

        public string NameIdentifier { get; set; }

        public string UserName { get; set; }

        public string SourceSystemUserName { get; set; }

        public string GivenName { get; set; }

        public string Surname { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public string Email { get; set; }

        public DateTimeOffset? DateAgreedToPrivacyPolicy { get; set; }

        public string PrivacyPolicyVersion { get; set; }

        public DateTimeOffset? DateAgreedToTermsOfService { get; set; }

        public string TermsOfServiceVersion { get; set; }

        public DateTimeOffset? MemberSince { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool PrefersMetric { get; set; }

        public bool IsOptedOutOfAllEmails { get; set; }

        public int TravelLimitForLocalEvents { get; set; }

        public bool IsSiteAdmin { get; set; }

        public bool IsTermsOfServiceAgreedTo
        {
            get
            {
                var isTermsOfServiceOutOfDate = DateAgreedToTermsOfService == null || DateAgreedToTermsOfService.Value < Constants.TermsOfServiceDate;

                return !isTermsOfServiceOutOfDate && !string.IsNullOrWhiteSpace(TermsOfServiceVersion);
            }
        }
        public bool IsPrivacyPolicyAgreedTo
        {
            get
            {
                var isPrivacyPolicyOutOfDate = DateAgreedToPrivacyPolicy == null || DateAgreedToPrivacyPolicy.Value < Constants.PrivacyPolicyDate;

                return !isPrivacyPolicyOutOfDate && !string.IsNullOrWhiteSpace(PrivacyPolicyVersion);
            }
        }
    }
}
