namespace TrashMob.Security
{
    using System.Collections.Generic;

    public class SecurityErrors
    {
        public List<SecurityError> errors { get; set; } = new List<SecurityError>();

        public void AddError(string message)
        {
            errors.Add(new SecurityError { message = message });
        }
    }

    public class SecurityError
    {
        public string message { get; set; }
    }
}
