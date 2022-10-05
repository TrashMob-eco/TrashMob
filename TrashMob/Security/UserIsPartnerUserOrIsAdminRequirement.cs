namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;

    public class UserIsPartnerUserOrIsAdminRequirement : IAuthorizationRequirement
    {
    }
}
