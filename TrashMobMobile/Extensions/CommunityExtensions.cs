namespace TrashMobMobile.Extensions;

using TrashMob.Models;
using TrashMobMobile.ViewModels;

public static class CommunityExtensions
{
    public static CommunityViewModel ToCommunityViewModel(this Partner partner)
    {
        var locationParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(partner.City))
        {
            locationParts.Add(partner.City);
        }

        if (!string.IsNullOrWhiteSpace(partner.Region))
        {
            locationParts.Add(partner.Region);
        }

        if (!string.IsNullOrWhiteSpace(partner.Country))
        {
            locationParts.Add(partner.Country);
        }

        return new CommunityViewModel
        {
            Id = partner.Id,
            Slug = partner.Slug ?? string.Empty,
            Name = partner.Name ?? string.Empty,
            Tagline = partner.Tagline ?? string.Empty,
            Location = string.Join(", ", locationParts),
            RegionTypeDisplay = GetRegionTypeLabel(partner.RegionType),
            HasRegionType = partner.RegionType.HasValue,
        };
    }

    private static string GetRegionTypeLabel(int? regionType)
    {
        return regionType switch
        {
            0 => "City",
            1 => "County",
            2 => "State",
            3 => "Province",
            4 => "Region",
            5 => "Country",
            _ => string.Empty,
        };
    }
}
