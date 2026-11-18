
using Cars.Core;
using Cars.WpfApp.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace Cars.WpfApp
{
    public partial class App : Application
    {
        private ServiceProvider? serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets<App>()
                .Build();

            string connectionString =
                configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string was not found.");

            ServiceCollection services = new ServiceCollection();

            services.AddSingleton(
                new DatabaseConnection(connectionString));

            services.AddSingleton<IWorldStorage>(provider =>
            {
                string storageType = configuration["Storage:Type"] ?? "SQL";

                return storageType.ToUpperInvariant() switch
                {
                    "JSON" => new JsonWorldStorage(),
                    "XML" => new XmlWorldStorage(),
                    "SQL" => new SqlWorldStorage(
                        provider.GetRequiredService<DatabaseConnection>()),

                    _ => throw new InvalidOperationException(
                        $"Unknown storage type: {storageType}")
                };
            });

            services.AddTransient<MainViewModel>();

            services.AddTransient<MainWindow>();

            serviceProvider = services.BuildServiceProvider();

            MainWindow mainWindow =
                serviceProvider.GetRequiredService<MainWindow>();

            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
