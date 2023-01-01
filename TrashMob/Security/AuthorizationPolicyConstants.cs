namespace TrashMob.Security
{
    public static class AuthorizationPolicyConstants
    {
        public const string UserIsAdmin = "UserIsAdmin";

        public const string UserIsPartnerUserOrIsAdmin = "UserIsPartnerUserOrIsAdmin";

        public const string UserOwnsEntity = "UserOwnsEntity";

        public const string UserOwnsEntityOrIsAdmin = "UserOwnsEntityOrIsAdmin";

        public const string ValidUser = "ValidUser";
    }
}
