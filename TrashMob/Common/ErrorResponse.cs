using System.Collections.Generic;

namespace TrashMob.Common
{
    public class ErrorResponse
    {
        public List<ErrorModel> Error { get; set; } = new List<ErrorModel>();
        public bool Successful { get; set; }
    }
}
