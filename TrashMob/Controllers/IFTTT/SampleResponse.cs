namespace TrashMob.Controllers.IFTTT
{
    using System.Collections.Generic;

    public class SampleResponse
    {
        public string accessToken { get; set; } = "XXX";

        public List<Sample> samples = new List<Sample>();

        public SampleResponse()
        {
            accessToken = "XXX";
            samples.Add(new Sample());
        }
    }
}
