namespace TrashMob.Controllers.IFTTT
{
    using System.Collections.Generic;

    public class SampleResponse
    {
        public string accessToken { get; set; } = "XXX";

        public List<Sample> samples { get; set; }
    }
}
