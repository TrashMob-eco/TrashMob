namespace TrashMobMobile.Models;

public class NewsletterPreferenceDto
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string CategoryDescription { get; set; } = string.Empty;

    public bool IsSubscribed { get; set; }

    public DateTimeOffset? SubscribedDate { get; set; }

    public DateTimeOffset? UnsubscribedDate { get; set; }
}
