namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for EmailInviteBatch.
    /// </summary>
    public static class EmailInviteBatchMappingsV2
    {
        /// <summary>
        /// Maps an EmailInviteBatch entity to an EmailInviteBatchDto.
        /// </summary>
        public static EmailInviteBatchDto ToV2Dto(this EmailInviteBatch entity)
        {
            return new EmailInviteBatchDto
            {
                Id = entity.Id,
                SenderUserId = entity.SenderUserId,
                BatchType = entity.BatchType,
                TotalCount = entity.TotalCount,
                SentCount = entity.SentCount,
                DeliveredCount = entity.DeliveredCount,
                BouncedCount = entity.BouncedCount,
                FailedCount = entity.FailedCount,
                Status = entity.Status,
                CompletedDate = entity.CompletedDate,
                CreatedDate = entity.CreatedDate,
            };
        }
    }
}
