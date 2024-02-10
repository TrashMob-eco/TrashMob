namespace TrashMobMobileApp.Authentication
{
    /// <summary>
    /// Simple platform specific service that is responsible for locating a 
    /// </summary>
    public interface IParentWindowLocatorService
    {
       object GetCurrentParentWindow();
    }
}
