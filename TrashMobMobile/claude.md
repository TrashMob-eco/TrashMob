# Claude AI Context for TrashMobMobile

## Project Overview
TrashMobMobile is a cross-platform mobile application built with .NET MAUI targeting .NET 10. The app supports community-driven trash cleanup events, allowing users to discover, register for, and manage events, create litter reports, track locations, and interact with other participants.

## Target Platforms
- Android (API 21+)
- iOS (15.0+)
- MacCatalyst (15.0+)

## Architecture & Patterns

### MVVM Pattern
- **ViewModels**: Located in `ViewModels/` folder. All ViewModels inherit from `BaseViewModel`.
- **Views**: XAML files in `Views/` folder for reusable content views.
- **Pages**: XAML files in `Pages/` folder for full-page navigation.
- **Models**: Data models in `Models/` folder and shared `TrashMob.Models` project.

### Key Conventions
1. **Property Change Notification**: Use `CommunityToolkit.Mvvm` attributes (`[ObservableProperty]`, `[RelayCommand]`)
2. **Async Operations**: All I/O operations use `async`/`await` pattern
3. **Dependency Injection**: Services injected via constructor
4. **Navigation**: Accessed via `Navigation` property on `BaseViewModel`
5. **Notifications**: Use `INotificationService` for user feedback

## Technology Stack

### Core Frameworks
- **.NET 10** with C# 14.0
- **MAUI Version**: 10.0.40
- **CommunityToolkit.Maui**: UI helpers and behaviors
- **CommunityToolkit.Mvvm**: MVVM helpers

### Key Packages
- `Microsoft.Maui.Controls.Maps`: Map integration
- `Microsoft.Identity.Client`: Authentication
- `Polly`: Resilience and fault-handling
- `Sentry.Maui`: Error tracking
- `Sharpnado.Tabs.Maui`: Tab controls

## Important Business Rules

### Event Management
- Users can only register for events if registration is enabled
- No duplicate registrations allowed
- Event capacity limits enforced
- End time must be after start time
- Only authorized users can edit/cancel events

### Litter Reports
- Maximum of 5 images per report (see `CreateLitterReportViewModel.MaxImages`)
- Name must be > 5 characters
- Description must be > 5 characters
- Each image must have location data (lat/long)
- Status ID 1 = New report

### Address & Location
- All addresses must be geocoded before use
- Location permissions required for mapping features
- Current location timeout: 10 seconds
- Location accuracy: Best available

## ViewModel Guidelines

### Base Class
All ViewModels should inherit from `BaseViewModel` which provides:
- `IsBusy` flag for loading states
- `Navigation` property for page navigation
- `NotificationService` for user notifications

### Property Patterns
```csharp
// Use ObservableProperty for simple properties
[ObservableProperty]
private string myProperty;

// Use full property with validation when needed
private string myValidatedProperty;
public string MyValidatedProperty
{
    get => myValidatedProperty;
    set
    {
        myValidatedProperty = value;
        OnPropertyChanged();
        ValidateForm(); // Custom validation
    }
}
```

### Command Patterns
```csharp
// Use RelayCommand for async operations
[RelayCommand]
private async Task ExecuteAction()
{
    IsBusy = true;
    try
    {
        // Implementation
        await NotificationService.Notify("Success");
    }
    catch (Exception ex)
    {
        SentrySdk.CaptureException(ex);
        await NotificationService.NotifyError("Error message");
    }
    finally
    {
        IsBusy = false;
    }
}
```

## Error Handling

### Standard Pattern
1. Wrap risky operations in try-catch
2. Log exceptions to Sentry: `SentrySdk.CaptureException(ex)`
3. Show user-friendly error via `NotificationService.NotifyError()`
4. Always reset `IsBusy` flag in finally block

### Location Errors
- Feature not supported
- Feature not enabled
- Permission denied
- Generic location failure

## Configuration

### Build Configurations
- **Debug**: Uses test environment (`USETEST` define)
- **Release**: Production environment

### Secrets Management
- User Secrets ID: `0a8c5383-6f9f-4b4e-a3f3-f8ac8cc4d258`
- Store API keys and sensitive config in user secrets

## Custom Controls

Located in `Controls/` folder:
- `CustomMap`: Map control with custom pins
- `CustomPin`: Map pin with custom visuals
- `TMDatePicker`: TrashMob-styled date picker
- `TMTimePicker`: TrashMob-styled time picker
- `TMPicker`: TrashMob-styled picker
- `TMEditor`: TrashMob-styled multi-line editor
- `TMEntry`: TrashMob-styled text entry

## Service Layer

### Manager Pattern
- `ILitterReportManager`: Business logic for litter reports
- `IEventManager`: Business logic for events
- Managers orchestrate REST services and data transformation

### REST Services
- `ILitterReportRestService`: API calls for litter reports
- `IMapRestService`: Geocoding and address lookup
- Use Polly for resilience (retries, circuit breaker)

## Testing Considerations

When writing or modifying code:
1. Ensure proper dependency injection for testability
2. Avoid platform-specific code in ViewModels
3. Use interfaces for all services
4. Keep business logic separate from UI logic
5. Validate all user inputs before API calls

## Common Anti-Patterns to Avoid

1. **Business logic in code-behind**: Keep in ViewModels/Services
2. **Direct UI manipulation**: Use data binding
3. **Synchronous I/O**: Always use async/await
4. **Hardcoded strings**: Use resources or constants
5. **Large ViewModels**: Split into focused, single-responsibility VMs
6. **Missing error handling**: Always handle exceptions gracefully
7. **Tight coupling**: Use DI and interfaces

## Code Style

- Use nullable reference types (`<Nullable>enable</Nullable>`)
- Use implicit usings (`<ImplicitUsings>enable</ImplicitUsings>`)
- Prefer `var` for obvious types
- Use expression-bodied members when appropriate
- Initialize collections with collection expressions: `[]` instead of `new()`

## Navigation Patterns

```csharp
// Push page onto navigation stack
await Navigation.PushAsync(new MyPage(viewModel));

// Pop current page
await Navigation.PopAsync();

// Navigate to shell route
await Shell.Current.GoToAsync("//myroute");
```

## Additional Resources

- Main repository: https://github.com/TrashMob-eco/TrashMob
- See `readme.md` for setup instructions
- Branch: `dev/jobee/addplan`

## When Making Changes

1. **Read existing code first**: Use appropriate tools to understand context
2. **Follow existing patterns**: Match the style and structure already in place
3. **Validate changes**: Ensure no compilation errors
4. **Test thoroughly**: Verify on target platforms
5. **Update documentation**: Keep this file and readme.md current
