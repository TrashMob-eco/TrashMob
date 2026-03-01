#nullable disable

namespace TrashMob.Models
{
    using System;

    /// <summary>
    /// Represents a task or checklist item associated with a grant.
    /// </summary>
    public class GrantTask : KeyedModel
    {
        public Guid GrantId { get; set; }

        public string Title { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public bool IsCompleted { get; set; }

        public DateTimeOffset? CompletedDate { get; set; }

        public int SortOrder { get; set; }

        public string Notes { get; set; }

        public virtual Grant Grant { get; set; }
    }
}
