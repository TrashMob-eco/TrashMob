namespace TrashMobMobile.Pages.CreateEvent;

public class BaseStepClass : ContentView
{
    public CreateEventViewModel ViewModel { get; set; }

    public event EventHandler NavigatedEvent;

    public virtual void OnNavigated()
    {
        NavigatedEvent?.Invoke(null,EventArgs.Empty);
    }
}