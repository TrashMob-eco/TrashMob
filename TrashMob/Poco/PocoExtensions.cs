
namespace TrashMob.Poco
{
    using TrashMob.Shared.Models;

    public static class PocoExtensions
    {
        public static DisplayUser ToDisplayUser(this User user)
        {
            return new DisplayUser
            {
                City = user.City,
                Country = user.Country,
                UserName = user.UserName,
                Email = user.Email,
                Id = user.Id,
                MemberSince = user.MemberSince,
                Region = user.Region            
            };
        }
    }
}
