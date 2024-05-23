namespace TrashMobMobile.Authentication
{
    public class UserState
    {
        public static UserContext UserContext { get; set; } = new();

        public static bool IsDeleting { get; set; } = false;
    }
}