# AppSync - .NET MAUI Project

This project is a **.NET MAUI** mobile application that demonstrates **Couchbase Lite** synchronization with **Couchbase Capella**. It allows users to view and manage employee profiles, with bidirectional synchronization to the backend.

## Directory Structure

The project follows the typical structure of a **.NET MAUI** application, with the following key directories:

- **`Models`**: Contains the data models used in the application, such as the `Profile` model representing employee information.
- **`Platforms`**: Contains platform-specific code for Android, iOS, etc.
- **`Properties`**: Contains application-level properties, such as settings and configurations.
- **`Resources`**: Contains image assets, fonts, and other resources for the application.
- **`Services`**: Contains services such as `CouchbaseService` for database operations and `SyncService` for synchronization.
- **`Views`**: Contains UI components and pages, such as `ProfilesPage`, where employee profiles are displayed.

## Key Files

- **`App.xaml`**: The entry point for the application, where application-wide resources are defined.
- **`App.xaml.cs`**: Handles the initialization of shared services like the `CouchbaseService` and `SyncService`.
- **`AppShell.xaml`**: Defines the navigation structure of the app (e.g., tabs, flyout menu).
- **`AppSync.csproj`**: The project file that contains references and settings for the .NET MAUI application.
- **`AppSync.sln`**: The solution file that contains the project and solution structure.
- **`MauiProgram.cs`**: The file where services and dependencies are configured for the app.
-  **`Dataset.json`**: This is the dataset you can import onto Capella.
