namespace TrashMobMobile.Extensions
{
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static bool IsValidEmailAddress(this string emailAddress)
        {
            var pattern = @"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

            var regex = new Regex(pattern);
            return regex.IsMatch(emailAddress);
        }
    }
}
