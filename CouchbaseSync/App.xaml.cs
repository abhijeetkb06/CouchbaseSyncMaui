namespace AppSync;
using Couchbase.Lite;
using AppSync.Services;

public partial class App : Application
{
    public static Database SharedDb { get; private set; } = null!;

    public App()
    {
        InitializeComponent();

        // Initialize shared DB instance
        var config = new DatabaseConfiguration();
        SharedDb = new Database("appsync-db", config);

        // Initialize CouchbaseService and SyncService with shared DB
        CouchbaseService.Instance.Initialize(SharedDb);
        _ = new SyncService(SharedDb); // Kick off sync
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
