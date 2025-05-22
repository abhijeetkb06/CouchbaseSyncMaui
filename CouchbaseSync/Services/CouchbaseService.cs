using System.Diagnostics;
using AppSync.Models;
using Couchbase.Lite;
using Couchbase.Lite.Query;
using System.Text.Json;
using System.IO;
using System.Reflection;
using Microsoft.Maui.Controls;

namespace AppSync.Services
{
   public class CouchbaseService
{
    public static CouchbaseService Instance { get; } = new CouchbaseService();

    public Database Db { get; private set; } = null!;
    private Collection _profileCollection = null!;
    private INavigation? _navigation;

    public void Initialize(Database sharedDb, INavigation? navigation = null)
    {
        _navigation = navigation;
        Db = sharedDb;
        EnsureCollectionExists("profiles", "employees");
        _profileCollection = Db.GetCollection("profiles", "employees");  // Cache collection
        Debug.WriteLine("CouchbaseService initialized with shared DB.");

        // Insert demo data if empty
        if (IsDatabaseEmpty())
        {
            InsertDemoProfiles();
        }
    }

    private async void ShowErrorToUser(string title, string message)
    {
        Debug.WriteLine($"Error: {title} - {message}");
        if (_navigation != null)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }
    }

    private bool IsDatabaseEmpty()
    {
        var query = QueryBuilder
            .Select(SelectResult.Expression(Meta.ID))
            .From(DataSource.Collection(_profileCollection))
            .Limit(Expression.Int(1));
        var resultSet = query.Execute();
        return !resultSet.Any();
    }

    private void InsertDemoProfiles()
    {
        var demoProfiles = new List<Profile>
        {
            new Profile { Id = "EMP0001", Name = "Danielle Johnson", Title = "Haematologist", Email = "danielle.johnson@acme.com" },
            new Profile { Id = "EMP0002", Name = "John Taylor", Title = "Civil engineer, consulting", Email = "john.taylor@acme.com" },
            new Profile { Id = "EMP0003", Name = "Erica Mcclain", Title = "Chartered loss adjuster", Email = "erica.mcclain@acme.com" },
            new Profile { Id = "EMP0004", Name = "Brittany Johnson", Title = "Chief Financial Officer", Email = "brittany.johnson@acme.com" },
            new Profile { Id = "EMP0005", Name = "Jeffery Wagner", Title = "Aid worker", Email = "jeffery.wagner@acme.com" }
        };
        foreach (var profile in demoProfiles)
        {
            SaveProfile(profile);
        }
        Debug.WriteLine($"Inserted {demoProfiles.Count} demo profiles.");
    }

    private void EnsureCollectionExists(string collectionName, string scopeName)
    {
        try
        {
            Db.CreateCollection(collectionName, scopeName);
            Debug.WriteLine($"Collection '{scopeName}.{collectionName}' created or already exists.");
        }
        catch (CouchbaseLiteException ex)
        {
            Debug.WriteLine($"Collection creation skipped: {ex.Message}");
        }
    }

    public List<Profile> GetAllProfiles()
    {
        var profiles = new List<Profile>();

        if (_profileCollection == null)
        {
            Debug.WriteLine("Collection not found.");
            return profiles;
        }

        try
        {
            var query = QueryBuilder
                .Select(
                    SelectResult.Expression(Meta.ID).As("id"),
                    SelectResult.Property("name"),
                    SelectResult.Property("title"),
                    SelectResult.Property("email"))
                .From(DataSource.Collection(_profileCollection));

            var resultSet = query.Execute();
            foreach (var row in resultSet)
            {
                profiles.Add(new Profile
                {
                    Id = row.GetString("id") ?? string.Empty,
                    Name = row.GetString("name") ?? string.Empty,
                    Title = row.GetString("title") ?? string.Empty,
                    Email = row.GetString("email") ?? string.Empty
                });
            }

            Debug.WriteLine($"Retrieved {profiles.Count} profile(s).");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Query error: {ex.Message}");
        }

        return profiles;
    }

    public void DeleteProfile(string id)
    {
        var doc = _profileCollection?.GetDocument(id);
        if (doc != null)
        {
            _profileCollection.Delete(doc);
            Debug.WriteLine($"Deleted document with ID: {id}");
        }
        else
        {
            Debug.WriteLine($"Document with ID {id} not found.");
        }
    }

    public void SaveProfile(Profile profile)
    {
        var doc = new MutableDocument(profile.Id);
        doc.SetString("name", profile.Name);
        doc.SetString("title", profile.Title);
        doc.SetString("email", profile.Email);
        _profileCollection.Save(doc);
        Debug.WriteLine($"Saved/updated document with ID: {profile.Id}");
    }
}

}
