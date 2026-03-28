namespace TrashMob.Models
{
    /// <summary>
    /// Defines the type of PRIVO consent request.
    /// </summary>
    public enum ConsentType
    {
        /// <summary>
        /// Adult identity verification via PRIVO (Flow 1).
        /// </summary>
        AdultVerification = 1,

        /// <summary>
        /// Parent-initiated consent for a 13-17 year-old child (Flow 2).
        /// </summary>
        ParentInitiatedChild = 2,

        /// <summary>
        /// Child-initiated consent request requiring parent approval (Flow 3).
        /// </summary>
        ChildInitiated = 3,
    }
}
