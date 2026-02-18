namespace TrashMobMobile.Tests.ViewModels;

using System.ComponentModel;
using TrashMob.Models;
using TrashMobMobile.ViewModels;
using Xunit;

public class EventViewModelTests
{
    [Fact]
    public void DisplayDuration_DefaultValues_ReturnsZero()
    {
        // Arrange
        var sut = new EventViewModel();

        // Assert
        Assert.Equal("0h 0m", sut.DisplayDuration);
    }

    [Fact]
    public void DisplayDuration_WithValues_ReturnsFormatted()
    {
        // Arrange
        var sut = new EventViewModel
        {
            DurationHours = 2,
            DurationMinutes = 30,
        };

        // Assert
        Assert.Equal("2h 30m", sut.DisplayDuration);
    }

    [Fact]
    public void DisplayDuration_UpdatesWhenHoursChange()
    {
        // Arrange
        var sut = new EventViewModel { DurationHours = 1, DurationMinutes = 0 };
        var changedProperties = new List<string>();
        sut.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName!);

        // Act
        sut.DurationHours = 3;

        // Assert
        Assert.Contains("DisplayDuration", changedProperties);
        Assert.Equal("3h 0m", sut.DisplayDuration);
    }

    [Fact]
    public void DisplayDuration_UpdatesWhenMinutesChange()
    {
        // Arrange
        var sut = new EventViewModel { DurationHours = 1, DurationMinutes = 0 };
        var changedProperties = new List<string>();
        sut.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName!);

        // Act
        sut.DurationMinutes = 45;

        // Assert
        Assert.Contains("DisplayDuration", changedProperties);
        Assert.Equal("1h 45m", sut.DisplayDuration);
    }

    [Fact]
    public void EventTime_Setter_CombinesDateAndTime()
    {
        // Arrange
        var sut = new EventViewModel
        {
            EventDate = new DateTimeOffset(2026, 3, 15, 0, 0, 0, TimeSpan.Zero),
        };

        // Act
        sut.EventTime = new TimeSpan(14, 30, 0);

        // Assert
        Assert.Equal(14, sut.EventDate.Hour);
        Assert.Equal(30, sut.EventDate.Minute);
        Assert.Equal(2026, sut.EventDate.Year);
        Assert.Equal(3, sut.EventDate.Month);
        Assert.Equal(15, sut.EventDate.Day);
    }

    [Fact]
    public void EventDateOnly_Setter_PreservesTime()
    {
        // Arrange
        var sut = new EventViewModel
        {
            EventDate = new DateTimeOffset(2026, 3, 15, 14, 30, 0, TimeSpan.Zero),
        };

        // Act
        sut.EventDateOnly = new DateTime(2026, 6, 20);

        // Assert
        Assert.Equal(2026, sut.EventDate.Year);
        Assert.Equal(6, sut.EventDate.Month);
        Assert.Equal(20, sut.EventDate.Day);
        Assert.Equal(14, sut.EventDate.Hour);
        Assert.Equal(30, sut.EventDate.Minute);
    }

    [Fact]
    public void ToEvent_MapsAllProperties()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var sut = new EventViewModel
        {
            Id = eventId,
            Name = "Beach Cleanup",
            Description = "Cleaning the beach",
            EventDate = new DateTimeOffset(2026, 7, 4, 9, 0, 0, TimeSpan.Zero),
            DurationHours = 3,
            DurationMinutes = 30,
            EventTypeId = 2,
            EventVisibilityId = (int)EventVisibilityEnum.TeamOnly,
            TeamId = teamId,
            MaxNumberOfParticipants = 25,
            EventStatusId = 1,
        };
        sut.Address.City = "Santa Monica";
        sut.Address.Region = "CA";
        sut.Address.Country = "United States";
        sut.Address.PostalCode = "90401";
        sut.Address.StreetAddress = "100 Pacific Ave";
        sut.Address.Latitude = 34.0195;
        sut.Address.Longitude = -118.4912;

        // Act
        var result = sut.ToEvent();

        // Assert
        Assert.Equal(eventId, result.Id);
        Assert.Equal("Beach Cleanup", result.Name);
        Assert.Equal("Cleaning the beach", result.Description);
        Assert.Equal(3, result.DurationHours);
        Assert.Equal(30, result.DurationMinutes);
        Assert.Equal(2, result.EventTypeId);
        Assert.Equal((int)EventVisibilityEnum.TeamOnly, result.EventVisibilityId);
        Assert.Equal(teamId, result.TeamId);
        Assert.Equal(25, result.MaxNumberOfParticipants);
        Assert.Equal("Santa Monica", result.City);
        Assert.Equal("CA", result.Region);
        Assert.Equal("United States", result.Country);
        Assert.Equal("90401", result.PostalCode);
        Assert.Equal("100 Pacific Ave", result.StreetAddress);
        Assert.Equal(34.0195, result.Latitude);
        Assert.Equal(-118.4912, result.Longitude);
    }

    [Fact]
    public void EventVisibilityText_Public_ReturnsPublic()
    {
        // Arrange
        var sut = new EventViewModel
        {
            EventVisibilityId = (int)EventVisibilityEnum.Public,
        };

        // Assert
        Assert.Equal("Public", sut.EventVisibilityText);
    }

    [Fact]
    public void EventVisibilityText_TeamOnly_ReturnsTeamOnly()
    {
        // Arrange
        var sut = new EventViewModel
        {
            EventVisibilityId = (int)EventVisibilityEnum.TeamOnly,
        };

        // Assert
        Assert.Equal("Team Only", sut.EventVisibilityText);
    }

    [Fact]
    public void IsValid_DefaultDate_ReturnsFalse()
    {
        // Arrange
        var sut = new EventViewModel
        {
            EventDate = DateTimeOffset.MinValue,
        };

        // Act
        var result = sut.IsValid();

        // Assert
        Assert.False(result);
        Assert.Equal("Event Date and Time must be specified.", sut.ErrorMessage);
    }

    [Fact]
    public void IsValid_WithValidDate_ReturnsTrue()
    {
        // Arrange
        var sut = new EventViewModel
        {
            EventDate = DateTimeOffset.Now.AddDays(1),
        };

        // Act
        var result = sut.IsValid();

        // Assert
        Assert.True(result);
    }
}
