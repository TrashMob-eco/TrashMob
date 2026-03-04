namespace TrashMobMobile.Controls;

using CommunityToolkit.Maui.Extensions;
using TrashMob.Models;

public partial class SelectDependentsPopup : ContentView
{
    private readonly List<DependentSelectItem> items;

    public SelectDependentsPopup(List<Dependent> dependents)
    {
        InitializeComponent();
        items = dependents.Select(d => new DependentSelectItem(d)).ToList();
        dependentsList.ItemsSource = items;
    }

    private void OnCheckChanged(object? sender, CheckedChangedEventArgs e)
    {
        var selectedCount = items.Count(i => i.IsSelected);
        registerButton.IsEnabled = selectedCount > 0;
        registerButton.Text = selectedCount > 0
            ? $"Register Selected ({selectedCount})"
            : "Register Selected";
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        var selectedIds = items
            .Where(i => i.IsSelected)
            .Select(i => i.Id)
            .ToList();

        await Shell.Current.ClosePopupAsync(selectedIds);
    }

    private async void OnSkipClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }
}

/// <summary>
/// Bindable wrapper for dependent selection in the popup.
/// </summary>
public class DependentSelectItem
{
    public DependentSelectItem(Dependent dependent)
    {
        Id = dependent.Id;
        DisplayName = $"{dependent.FirstName} {dependent.LastName}";
        Relationship = dependent.Relationship;
    }

    public Guid Id { get; }

    public string DisplayName { get; }

    public string Relationship { get; }

    public bool IsSelected { get; set; }
}
