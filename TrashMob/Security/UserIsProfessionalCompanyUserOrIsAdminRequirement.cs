namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;

    public class UserIsProfessionalCompanyUserOrIsAdminRequirement : IAuthorizationRequirement
    {
    }
}
