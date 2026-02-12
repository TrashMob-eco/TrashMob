namespace TrashMobMobile.UITests.Setup;

using Xunit;

[CollectionDefinition(Name)]
public class TestCollection : ICollectionFixture<AppiumFixture>
{
    public const string Name = "Appium";
}
