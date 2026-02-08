namespace TrashMobMobile.Models;

public class NewsletterCategoryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}
