namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Requirement for the <see cref="AuthorizationPolicyConstants.UserIsSalesRepOrIsAdmin"/>
    /// policy. Satisfied when the caller has the SalesRep or SiteAdmin role
    /// (see <see cref="TrashMob.Shared.Managers.RoleNames"/>).
    /// </summary>
    public class UserIsSalesRepOrIsAdminRequirement : IAuthorizationRequirement
    {
    }
}
