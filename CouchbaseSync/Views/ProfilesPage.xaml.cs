using AppSync.Models;
using AppSync.Services;
using System.Collections.ObjectModel;

namespace AppSync.Views;

public partial class ProfilesPage : ContentPage
{
    public ObservableCollection<Profile> Profiles { get; set; } = new();
    public Command<Profile> EditCommand { get; }
    public Command<Profile> DeleteCommand { get; }

    public ProfilesPage()
    {
        InitializeComponent();
        BindingContext = this;

        EditCommand = new Command<Profile>(EditProfile);
        DeleteCommand = new Command<Profile>(DeleteProfile);

        LoadProfiles();
    }

    private async void LoadProfiles()
{
    var docs = await Task.Run(() => CouchbaseService.Instance.GetAllProfiles()); // Async call to get profiles
    Profiles.Clear();
    foreach (var doc in docs)
    {
        Profiles.Add(doc);
    }
}


    private async void EditProfile(Profile profile)
    {
        // Just a stub for now — we can build EditPage next
        await DisplayAlert("Edit", $"Would edit: {profile.Name}", "OK");
    }

    private async void DeleteProfile(Profile profile)
    {
        var confirm = await DisplayAlert("Delete", $"Delete {profile.Name}?", "Yes", "No");
        if (confirm)
        {
            CouchbaseService.Instance.DeleteProfile(profile.Id);
            Profiles.Remove(profile);
        }
    }
}
