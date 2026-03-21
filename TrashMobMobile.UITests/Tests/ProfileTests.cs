namespace TrashMobMobile.UITests.Tests;

using TrashMobMobile.UITests.Setup;
using Xunit;

/// <summary>
/// Tests for the Profile page (authenticated).
/// Verifies user details, email, and sign out functionality.
/// </summary>
[Collection(TestCollection.Name)]
[Trait("Category", "Authenticated")]
public class ProfileTests : BaseTest
{
    public ProfileTests(AppiumFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void Profile_DisplaysUserName()
    {
        TapElement("ProfileTab");
        var userName = WaitForElement("UserNameLabel");
        Assert.True(userName.Displayed);
        Assert.False(string.IsNullOrWhiteSpace(userName.Text));

        TapElement("HomeTab");
    }

    [Fact]
    public void Profile_DisplaysEmail()
    {
        TapElement("ProfileTab");
        var email = WaitForElement("UserEmailLabel");
        Assert.True(email.Displayed);
        Assert.Contains("@", email.Text);

        TapElement("HomeTab");
    }

    [Fact]
    public void Profile_DisplaysMemberSince()
    {
        TapElement("ProfileTab");
        var memberSince = WaitForElement("MemberSinceLabel");
        Assert.True(memberSince.Displayed);
        Assert.False(string.IsNullOrWhiteSpace(memberSince.Text));

        TapElement("HomeTab");
    }

    [Fact]
    public void Profile_SignOutButtonVisible()
    {
        TapElement("ProfileTab");
        var signOut = WaitForElement("SignOutButton");
        Assert.True(signOut.Displayed);

        TapElement("HomeTab");
    }
}
