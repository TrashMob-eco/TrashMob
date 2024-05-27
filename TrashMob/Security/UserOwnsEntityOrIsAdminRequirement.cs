namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;

    public class UserOwnsEntityOrIsAdminRequirement : IAuthorizationRequirement
    {
    }
}