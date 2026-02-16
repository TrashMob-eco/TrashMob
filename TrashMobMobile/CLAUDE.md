# TrashMobMobile — AI Assistant Context

> **Note:** For overall project context and coding standards, see [/CLAUDE.md](../CLAUDE.md) at the repository root. This document covers patterns specific to the .NET MAUI mobile app.

## Application Overview

Cross-platform mobile app built with **.NET MAUI** targeting .NET 10, using **CommunityToolkit.Mvvm** for MVVM pattern with source generators and **Sentry.io** for crash reporting.

## Project Structure

```
TrashMobMobile/
├── ViewModels/          # MVVM ViewModels (partial classes with source generators)
│   ├── BaseViewModel.cs # Base class: IsBusy, IsError, ExecuteAsync()
│   └── ...              # One ViewModel per feature screen
├── Pages/               # XAML pages with code-behind
├── Views/               # Reusable XAML views/controls
├── Controls/            # Custom controls (map pins, etc.)
├── Models/              # Data models and DTOs
├── Services/            # REST API clients and local services
│   ├── Interfaces/      # Service interfaces
│   └── ...              # Implementations (REST + auth + notifications)
├── Platforms/           # Platform-specific code (Android, iOS, Mac)
├── Config/              # Constants, auth config
├── Extensions/          # Extension methods
├── App.xaml.cs          # Application entry point
├── AppShell.xaml.cs     # Shell routing registration
└── MauiProgram.cs       # DI container setup
```

## Build & Run

```bash
# Build for Android
dotnet build TrashMobMobile -f net10.0-android -p:AndroidSdkDirectory=D:\\Android\\android-sdk

# Build for iOS (macOS only)
dotnet build TrashMobMobile -f net10.0-ios

# Run on Android emulator
dotnet build TrashMobMobile -f net10.0-android -t:Run
```

## ViewModel Pattern

### BaseViewModel

All ViewModels extend `BaseViewModel` which provides:
- **`IsBusy`** — Bindable loading state
- **`IsError`** — Bindable error state
- **`Navigation`** — Set by Page code-behind for programmatic navigation
- **`ExecuteAsync(operation, errorMessage)`** — Wraps async operations with error handling, Sentry capture, and connectivity checks

### Standard ViewModel Template

```csharp
// Use primary constructors + partial class for CommunityToolkit source generation
public partial class ThingViewModel(
    IThingService thingService,
    INotificationService notificationService)
    : BaseViewModel(notificationService)
{
    // Bindable properties — [ObservableProperty] generates public property + change notification
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private ObservableCollection<ThingData> items = [];

    // Commands — [RelayCommand] generates ICommand property (ThingCommand from DoThing)
    [RelayCommand]
    private async Task LoadData()
    {
        await ExecuteAsync(async () =>
        {
            var result = await thingService.GetAllAsync();
            Items = new ObservableCollection<ThingData>(result);
        }, "Failed to load items.");
    }

    [RelayCommand]
    private async Task Save()
    {
        await ExecuteAsync(async () =>
        {
            await thingService.UpdateAsync(currentItem);
            await NotificationService.Notify("Saved successfully!");
        }, "Failed to save.");
    }

    // Navigation
    [RelayCommand]
    private async Task GoToDetail()
    {
        await Shell.Current.GoToAsync($"{nameof(ThingDetailPage)}?ThingId={selectedItem.Id}");
    }

    // Initialization method called from Page.OnNavigatedTo
    public async Task Init(Guid id)
    {
        await LoadData();
    }
}
```

### ViewModel Conventions

- Always use **`partial class`** (required for CommunityToolkit source generators)
- Use **`[ObservableProperty]`** for all bindable fields (generates public properties)
- Use **`[RelayCommand]`** for all commands (generates `ICommand` properties)
- Use **`[RelayCommand(IncludeCancelCommand = true)]`** for long-running operations that need cancellation
- Wrap all async operations in **`ExecuteAsync()`** for consistent error handling
- Field names are `camelCase` private; generated properties are `PascalCase` public
- Initialize data in an **`Init()`** method, not the constructor (called from `OnNavigatedTo`)
- Use **`ObservableCollection<T>`** for lists bound to CollectionView

### Common ViewModel Pitfalls

- **Don't load data in constructor** — Pages are created by DI before navigation completes; use `Init()` called from `OnNavigatedTo`
- **Don't forget `partial`** — Without it, `[ObservableProperty]` and `[RelayCommand]` silently do nothing
- **Don't use `async void`** except in `OnNavigatedTo` — Use `[RelayCommand]` + `async Task` instead
- **Don't catch exceptions manually** — Use `ExecuteAsync()` which handles Sentry + user notification

## Page Pattern

### Standard Page Template

```csharp
// Code-behind
[QueryProperty(nameof(ThingId), nameof(ThingId))]
public partial class ThingPage : ContentPage
{
    private readonly ThingViewModel viewModel;

    public ThingPage(ThingViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    // Navigation parameter — set by Shell before OnNavigatedTo
    public string ThingId { get; set; } = string.Empty;

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init(new Guid(ThingId));
    }
}
```

### Page Conventions

- Use **`[QueryProperty]`** for navigation parameters (Shell sets these before `OnNavigatedTo`)
- **Inject ViewModel** via constructor (registered as `AddTransient` in DI)
- Set **`BindingContext = viewModel`** in constructor
- Set **`viewModel.Navigation = Navigation`** so ViewModel can navigate
- Call **`viewModel.Init()`** in `OnNavigatedTo` (not constructor — navigation params aren't set yet)
- All pages must be `partial class` for XAML code generation

## Adding a New Screen (Step-by-Step)

### 1. Create the ViewModel

```csharp
// ViewModels/NewFeatureViewModel.cs
public partial class NewFeatureViewModel(
    IFeatureService featureService,
    INotificationService notificationService)
    : BaseViewModel(notificationService)
{
    [ObservableProperty]
    private string title = string.Empty;

    public async Task Init(Guid id)
    {
        await ExecuteAsync(async () =>
        {
            var data = await featureService.GetAsync(id);
            Title = data.Title;
        }, "Failed to load feature.");
    }
}
```

### 2. Create the XAML Page

```xml
<!-- Pages/NewFeaturePage.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="TrashMobMobile.Pages.NewFeaturePage"
             Title="New Feature">
    <ScrollView>
        <VerticalStackLayout Padding="16" Spacing="12">
            <ActivityIndicator IsRunning="{Binding IsBusy}" IsVisible="{Binding IsBusy}" />
            <Label Text="{Binding Title}" FontSize="24" FontAttributes="Bold" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

### 3. Create the Code-Behind

```csharp
// Pages/NewFeaturePage.xaml.cs
[QueryProperty(nameof(FeatureId), nameof(FeatureId))]
public partial class NewFeaturePage : ContentPage
{
    private readonly NewFeatureViewModel viewModel;

    public NewFeaturePage(NewFeatureViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    public string FeatureId { get; set; } = string.Empty;

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init(new Guid(FeatureId));
    }
}
```

### 4. Register in DI (`MauiProgram.cs`)

```csharp
// Add both ViewModel and Page as transient services
builder.Services.AddTransient<NewFeatureViewModel>();
builder.Services.AddTransient<NewFeaturePage>();
```

### 5. Register Shell Route (`AppShell.xaml.cs`)

```csharp
Routing.RegisterRoute(nameof(NewFeaturePage), typeof(NewFeaturePage));
```

### 6. Navigate to the New Page

```csharp
// From any ViewModel
await Shell.Current.GoToAsync($"{nameof(NewFeaturePage)}?FeatureId={featureId}");
```

## Service Pattern

REST API clients follow a simple interface + implementation pattern:

```csharp
// Services/Interfaces/IFeatureService.cs
public interface IFeatureService
{
    Task<FeatureData> GetAsync(Guid id);
    Task<IEnumerable<FeatureData>> GetAllAsync();
    Task<FeatureData> CreateAsync(FeatureData data);
    Task<FeatureData> UpdateAsync(FeatureData data);
    Task DeleteAsync(Guid id);
}
```

Services are registered in `MauiProgram.cs`:
```csharp
builder.Services.AddSingleton<IFeatureService, FeatureRestService>();
```

## Error Handling

- **All async operations** should use `BaseViewModel.ExecuteAsync()` — it catches exceptions, reports to Sentry, shows user-friendly notifications, and manages `IsBusy`/`IsError` state
- **Network errors** are detected with `Connectivity.Current.NetworkAccess` and show a specific "No internet" message
- **Timeout errors** (`TaskCanceledException`) show a specific "Request timed out" message
- **Other errors** show the custom error message passed to `ExecuteAsync()`
- **Never swallow exceptions** — always let `ExecuteAsync` handle them for consistent Sentry reporting

## Testing

ViewModel tests use xUnit with Moq:

```csharp
public class ThingViewModelTests
{
    private readonly Mock<IThingService> mockService = new();
    private readonly Mock<INotificationService> mockNotification = new();

    private ThingViewModel CreateViewModel() =>
        new(mockService.Object, mockNotification.Object);

    [Fact]
    public async Task Init_LoadsData()
    {
        var expected = new ThingData { Id = Guid.NewGuid(), Name = "Test" };
        mockService.Setup(s => s.GetAsync(expected.Id)).ReturnsAsync(expected);

        var vm = CreateViewModel();
        await vm.Init(expected.Id);

        Assert.Equal("Test", vm.Name);
        Assert.False(vm.IsBusy);
    }
}
```

Test files are in `TrashMob.Shared.Tests/` alongside backend tests (the mobile ViewModels are tested there since they depend on shared interfaces).

---

**Related Documentation:**
- [Root CLAUDE.md](../CLAUDE.md) — Architecture, patterns, coding standards
- [readme.md](./readme.md) — Build instructions, project overview
- [TrashMobMobile.prd](./TrashMobMobile.prd) — Product requirements
- [Project 4 - Mobile Robustness](../Planning/Projects/Project_04_Mobile_Robustness.md) — Stabilization roadmap
- [Project 38 - Mobile Feature Parity](../Planning/Projects/Project_38_Mobile_Feature_Parity.md) — Feature parity tracker

**Last Updated:** February 15, 2026
