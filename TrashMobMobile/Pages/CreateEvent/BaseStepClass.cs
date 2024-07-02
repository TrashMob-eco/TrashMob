namespace TrashMobMobile.Pages.CreateEvent;

public class BaseStepClass : ContentView
{
    public CreateEventViewModelNew ViewModel { get; set; }

    public event EventHandler NavigatedEvent;

    public void OnNavigated()
    {
        NavigatedEvent?.Invoke(null,EventArgs.Empty);
    }
}