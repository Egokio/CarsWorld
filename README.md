# Cars World

## Project Description

Cars World is a WPF desktop application developed in C# and .NET.

The application simulates a world of different cars. Users can create and manage cars, perform actions, organize cars into groups, and save or load the entire world.

The application supports three storage methods:
- JSON file storage
- XML file storage
- SQL Server LocalDB database storage

## Technologies

- C#
- .NET
- WPF (Windows Presentation Foundation)
- SQL Server LocalDB
- ADO.NET
- Microsoft.Data.SqlClient
- System.Text.Json
- XML Serialization
- Dependency Injection

## Project Structure

The solution contains two main projects:

- **Cars.Core** – contains the business logic, car classes, repositories, groups, DTOs, validation, mapping, and storage implementations.
- **Cars.WpfApp** – contains the WPF user interface, ViewModels, application startup, and dependency injection configuration.

## Requirements

Before running the application, make sure you have:

- Windows
- Visual Studio 2026 or a compatible version
- .NET SDK required by the solution
- SQL Server Express LocalDB (required only for SQL storage)

## How to Run

1. Clone or download the repository.
2. Open `CarsWorld.slnx` in Visual Studio.
3. Restore NuGet packages.
4. Configure the storage type in User Secrets.
5. Configure the database connection if SQL storage is selected.
6. Build the solution.
7. Run the `Cars.WpfApp` project.

## Database Setup (SQL Server LocalDB)

The application uses SQL Server LocalDB for database storage.

### 1. Install LocalDB

Install SQL Server Express LocalDB if it is not already installed.

### 2. Create the Database

Open SQL Server Object Explorer in Visual Studio and connect to:

`(localdb)\MSSQLLocalDB`

Create a database named:

`CarsWorldDb`

### 3. Create the Tables

The database requires three tables:

- `Cars` – stores car information and type-specific properties.
- `CarGroups` – stores car groups.
- `CarGroupMembers` – stores relationships between cars and groups.

The `CarGroupMembers` table uses foreign keys to connect cars and groups. A car can belong to multiple groups.


## Configuration

The application uses User Secrets to store local configuration values.

To configure User Secrets:

1. Right-click the `Cars.WpfApp` project in Visual Studio.
2. Select **Manage User Secrets**.
3. Add the following configuration:

```json
{
  "ConnectionStrings": {
    "DatabaseConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CarsWorldDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Storage": {
    "Type": "SQL"
  }
}
```

The connection string is required when SQL storage is selected.

User Secrets keeps local connection settings outside the source code repository.

## Storage Selection

The storage implementation is selected during application startup using the `Storage:Type` setting in User Secrets.

Available options:

| Value | Storage implementation |
|---|---|
| `JSON` | JsonWorldStorage |
| `XML` | XmlWorldStorage |
| `SQL` | SqlWorldStorage |

To change the storage method, update the `Storage:Type` value.

For example, to use JSON:

```json
"Storage": {
  "Type": "JSON"
}
```

To use XML:

```json
"Storage": {
  "Type": "XML"
}
```

To use SQL Server LocalDB:

```json
"Storage": {
  "Type": "SQL"
}
```

Restart the application after changing the setting.

## Saving and Loading the World

The application can save and restore the complete car world.

The saved data includes:

- Car IDs and concrete car types
- Car names and models
- Speed, fuel, and condition
- Type-specific properties (turbo level, tow capacity, and siren state)
- Reaction counts
- Group IDs and names
- Car-to-group relationships

When loading a world, the application recreates the correct car types and restores their original IDs and state.

The application validates loaded data before replacing the current world. If loading fails, the existing world remains unchanged.

## Error Handling

The application handles invalid or corrupted saved data and database errors.

If loading fails, an error message is added to the application log. The current world is preserved.

## Main Features

- Create and manage different car types.
- Perform car-specific actions.
- Organize cars into groups.
- Add one car to multiple groups.
- Save and load the world using JSON, XML, or SQL.
- Restore object identities and relationships.
- Use repositories and dependency injection.
