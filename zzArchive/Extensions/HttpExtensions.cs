namespace TrashMobMobileApp.Extensions
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Temporary solution to avoid closed stream error. 
        /// Fix going forward, httpresponse dispose issue with streams.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static bool IsClosedStreamException(this Exception exception)
            => exception?.Message?.Contains("Cannot access a closed Stream", StringComparison.OrdinalIgnoreCase) ?? false;
    }
}
