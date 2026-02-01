namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Authorization requirement that checks if the user is an event lead.
    /// An event lead is either the event creator or has been promoted to co-lead status.
    /// </summary>
    public class UserIsEventLeadRequirement : IAuthorizationRequirement
    {
    }
}
