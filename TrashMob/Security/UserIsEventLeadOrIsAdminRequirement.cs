namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Authorization requirement that checks if the user is an event lead or a site admin.
    /// </summary>
    public class UserIsEventLeadOrIsAdminRequirement : IAuthorizationRequirement
    {
    }
}
