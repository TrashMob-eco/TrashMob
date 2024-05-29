namespace TrashMob.Controllers.IFTTT
{
    using System.Collections.Generic;

    public class ErrorResponse
    {
        public List<Error> Errors { get; } = new();
    }
}