using Couchbase.Lite;
using Couchbase.Lite.Sync;
using System;
using System.Diagnostics;

namespace AppSync.Services
{
    public class SyncService
    {
        private readonly Database _database;
        private readonly Replicator _replicator;

        public SyncService(Database sharedDb)
        {
            _database = sharedDb;

            var collection = _database.GetCollection("profiles", "employees");
            if (collection == null)
            {
                throw new InvalidOperationException("Collection 'profiles' in scope 'employees' does not exist.");
            }

            var syncGatewayUrl = new Uri("wss://988vxq3nsjinnxsy.apps.cloud.couchbase.com:4984/app-endpoint");
            var target = new URLEndpoint(syncGatewayUrl);

            var config = new ReplicatorConfiguration(target)
            {
                ReplicatorType = ReplicatorType.PushAndPull,
                Continuous = true,
                Authenticator = new BasicAuthenticator("test-user", "Appservices@123")
            };

            config.AddCollection(collection);

            _replicator = new Replicator(config);
            _replicator.AddChangeListener(OnReplicatorStatusChanged);
            _replicator.AddDocumentReplicationListener(OnDocumentReplication);

            Debug.WriteLine("Starting replicator...");
            _replicator.Start();
        }

        private void OnReplicatorStatusChanged(object? sender, ReplicatorStatusChangedEventArgs e)
        {
            var status = e.Status;
            Debug.WriteLine($"[Sync] Status: {status.Activity}, Completed: {status.Progress.Completed}, Total: {status.Progress.Total}");

            if (status.Error != null)
            {
                Debug.WriteLine($"[Sync] Error: {status.Error}");
                // Implement retry logic or exponential backoff here.
            }
        }

        private void OnDocumentReplication(object? sender, DocumentReplicationEventArgs e)
        {
            var direction = e.IsPush ? "Push" : "Pull";
            Debug.WriteLine($"[Sync] {direction} - {e.Documents.Count} documents");

            foreach (var doc in e.Documents)
            {
                if (doc.Error == null)
                {
                    Debug.WriteLine($"[Sync] Synced document ID: {doc.Id}");
                }
                else
                {
                    Debug.WriteLine($"[Sync] Error syncing doc {doc.Id}: {doc.Error}");
                    // Implement retry or notify the user on sync errors
                }
            }
        }

        public Collection GetCollection() => _database.GetCollection("profiles", "employees")
                                              ?? throw new InvalidOperationException("Collection not found.");

        public Database GetDatabase() => _database;
    }
}
