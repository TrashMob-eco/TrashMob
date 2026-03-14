#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// V2 API request body for flagging a photo.
    /// </summary>
    public class FlagPhotoRequestDto
    {
        /// <summary>Gets or sets the reason for flagging.</summary>
        public string Reason { get; set; } = string.Empty;
    }
}
