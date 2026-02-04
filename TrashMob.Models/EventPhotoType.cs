#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents the timing of when an event photo was taken.
    /// </summary>
    public enum EventPhotoType
    {
        /// <summary>
        /// Photo taken before the cleanup event started.
        /// </summary>
        Before = 0,

        /// <summary>
        /// Photo taken during the cleanup event.
        /// </summary>
        During = 1,

        /// <summary>
        /// Photo taken after the cleanup event completed.
        /// </summary>
        After = 2
    }
}
