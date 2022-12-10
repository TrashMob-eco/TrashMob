
namespace TrashMob.Models
{
    public class Waiver : KeyedModel
    {
        public string Name { get; set; } = string.Empty;

        public bool IsWaiverEnabled { get; set; } = true;
    }
}
