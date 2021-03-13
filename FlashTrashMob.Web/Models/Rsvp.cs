namespace FlashTrashMob.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;

    public class Rsvp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RsvpId { get; set; }

        [Required]
        public long CleanupEventId { get; set; }

        [Required]
        [MaxLength(64)]
        public string UserName { get; set; }

        [JsonIgnore]
        public CleanupEvent CleanupEvent { get; set; }
    }
}
