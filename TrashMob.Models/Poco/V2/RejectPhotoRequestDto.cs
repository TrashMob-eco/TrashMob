#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// V2 API request body for rejecting a photo.
    /// </summary>
    public class RejectPhotoRequestDto
    {
        /// <summary>Gets or sets the rejection reason.</summary>
        public string Reason { get; set; } = string.Empty;
    }
}
