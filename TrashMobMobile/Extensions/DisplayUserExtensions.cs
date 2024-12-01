namespace TrashMobMobile.Extensions
{
    using TrashMob.Models.Poco;

    public static class DisplayUserExtensions
    {
        public static EventAttendeeViewModel ToEventAttendeeViewModel(this DisplayUser displayUser)
        {
            return new EventAttendeeViewModel
            {
                MemberSince = displayUser.MemberSince?.GetFormattedLocalDate() ?? string.Empty,
                UserName = displayUser.UserName ?? string.Empty                
            };
        }
    }
}