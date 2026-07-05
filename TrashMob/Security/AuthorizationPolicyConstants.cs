namespace TrashMob.Security
{
    public static class AuthorizationPolicyConstants
    {
        public const string UserIsAdmin = "UserIsAdmin";

        public const string UserIsPartnerUserOrIsAdmin = "UserIsPartnerUserOrIsAdmin";

        public const string UserOwnsEntity = "UserOwnsEntity";

        public const string UserOwnsEntityOrIsAdmin = "UserOwnsEntityOrIsAdmin";

        public const string UserIsEventLead = "UserIsEventLead";

        public const string UserIsEventLeadOrIsAdmin = "UserIsEventLeadOrIsAdmin";

        public const string ValidUser = "ValidUser";

        public const string UserIsProfessionalCompanyUserOrIsAdmin = "UserIsProfessionalCompanyUserOrIsAdmin";

        /// <summary>
        /// Authorizes users with the SalesRep role (Project 63 — municipal sales
        /// pipeline access) or the SiteAdmin role. Applied to the prospect and
        /// sales-report v2 controllers.
        /// </summary>
        public const string UserIsSalesRepOrIsAdmin = "UserIsSalesRepOrIsAdmin";

        public const string IftttServiceKey = "IftttServiceKey";
    }
}