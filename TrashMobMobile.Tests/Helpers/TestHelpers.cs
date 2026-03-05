namespace TrashMobMobile.Tests.Helpers;

using TrashMob.Models;
using TrashMob.Models.Poco;

/// <summary>
/// Factory methods for creating test data.
/// </summary>
internal static class TestHelpers
{
    public static User CreateTestUser(
        Guid? id = null,
        string userName = "TestUser",
        string email = "test@example.com",
        string city = "Seattle",
        string region = "WA",
        string country = "United States",
        double latitude = 47.6062,
        double longitude = -122.3321)
    {
        return new User
        {
            Id = id ?? Guid.NewGuid(),
            UserName = userName,
            Email = email,
            City = city,
            Region = region,
            Country = country,
            Latitude = latitude,
            Longitude = longitude,
            TravelLimitForLocalEvents = 20,
            DateAgreedToTrashMobWaiver = DateTime.UtcNow,
        };
    }

    public static Event CreateTestEvent(
        Guid? id = null,
        string name = "Test Cleanup",
        DateTimeOffset? eventDate = null,
        string city = "Seattle",
        string region = "WA",
        string country = "United States",
        Guid? createdByUserId = null,
        int eventStatusId = 1,
        bool isEventPublic = true)
    {
        return new Event
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            Description = "Test event description",
            EventDate = eventDate ?? DateTimeOffset.UtcNow.AddDays(7),
            City = city,
            Region = region,
            Country = country,
            StreetAddress = "123 Main St",
            PostalCode = "98101",
            Latitude = 47.6062,
            Longitude = -122.3321,
            CreatedByUserId = createdByUserId ?? Guid.NewGuid(),
            EventStatusId = eventStatusId,
            EventVisibilityId = isEventPublic ? (int)EventVisibilityEnum.Public : (int)EventVisibilityEnum.Private,
            EventTypeId = 1,
            DurationHours = 2,
            DurationMinutes = 0,
            MaxNumberOfParticipants = 50,
        };
    }

    public static List<Event> CreateTestEvents(int count, string country = "United States", string region = "WA", string city = "Seattle")
    {
        var events = new List<Event>();
        for (var i = 0; i < count; i++)
        {
            events.Add(CreateTestEvent(
                name: $"Event {i + 1}",
                eventDate: DateTimeOffset.UtcNow.AddDays(i + 1),
                city: city,
                region: region,
                country: country));
        }

        return events;
    }

    public static PaginatedList<Event> ToPaginatedList(this IEnumerable<Event> events)
    {
        var list = new PaginatedList<Event>();
        list.AddRange(events);
        return list;
    }

    public static List<Location> CreateTestLocations()
    {
        return
        [
            new Location { Country = "United States", Region = "WA", City = "Seattle" },
            new Location { Country = "United States", Region = "WA", City = "Tacoma" },
            new Location { Country = "United States", Region = "OR", City = "Portland" },
            new Location { Country = "Canada", Region = "BC", City = "Vancouver" },
        ];
    }

    public static Stats CreateTestStats()
    {
        return new Stats
        {
            TotalParticipants = 1000,
            TotalBags = 5000,
            TotalEvents = 200,
            TotalHours = 3000,
            TotalLitterReportsSubmitted = 500,
        };
    }

    public static Dependent CreateTestDependent(
        Guid? id = null,
        Guid? parentUserId = null,
        string firstName = "Junior",
        string lastName = "Tester",
        DateOnly? dateOfBirth = null,
        string relationship = "Parent")
    {
        return new Dependent
        {
            Id = id ?? Guid.NewGuid(),
            ParentUserId = parentUserId ?? Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth ?? DateOnly.FromDateTime(DateTime.Today.AddYears(-8)),
            Relationship = relationship,
            IsActive = true,
        };
    }

    public static List<Dependent> CreateTestDependents(int count, Guid? parentUserId = null)
    {
        var parent = parentUserId ?? Guid.NewGuid();
        var dependents = new List<Dependent>();
        for (var i = 0; i < count; i++)
        {
            dependents.Add(CreateTestDependent(
                parentUserId: parent,
                firstName: $"Child{i + 1}",
                lastName: "Tester"));
        }

        return dependents;
    }

    public static LitterReport CreateTestLitterReport(Guid? id = null)
    {
        return new LitterReport
        {
            Id = id ?? Guid.NewGuid(),
            Name = "Test Litter Report",
            Description = "Litter at intersection",
            LitterReportStatusId = (int)LitterReportStatusEnum.New,
            CreatedDate = DateTimeOffset.UtcNow,
            LitterImages = [],
        };
    }
}
