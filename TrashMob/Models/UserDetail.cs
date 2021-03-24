using System;
using System.Collections.Generic;

#nullable disable

namespace TrashMob.Models
{
    public partial class UserDetail
    {
        public string UserId { get; set; }
        public DateTimeOffset? DateAgreedToPrivacyPolicy { get; set; }
        public string PrivacyPolicyVersion { get; set; }
        public DateTimeOffset? DateAgreedToTermsOfService { get; set; }
        public string TermsOfServiceVersion { get; set; }
        public string RecruitedByUserId { get; set; }
        public DateTimeOffset? MemberSince { get; set; }

        public virtual AspNetUser RecruitedByUser { get; set; }
        public virtual AspNetUser User { get; set; }
    }
}
