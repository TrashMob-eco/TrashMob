namespace TrashMob.Controllers.IFTTT
{
    using DocuSign.eSign.Model;
    using System.Collections.Generic;

    public class SampleResponse
    {
        public string accessToken { get; set; } = string.Empty;

        public List<Sample> samples = new List<Sample>();
    }
}
