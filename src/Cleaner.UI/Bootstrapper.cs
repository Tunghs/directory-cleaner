using Cleaner.UI.Viewers;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Cleaner.UI
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            var services = ConfigureServices();
            Ioc.Default.ConfigureServices(services);
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        /// <returns></returns>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services

            // ViewModels
            services.AddTransient<ShellViewModel>();
            services.AddTransient<SettingViewModel>();

            return services.BuildServiceProvider();
        }
    }
}