namespace TrashMob.Common
{
    using System.Collections.Generic;

    public class ErrorResponse
    {
        public List<ErrorModel> Error { get; set; } = new();
        public bool Successful { get; set; }
    }
}