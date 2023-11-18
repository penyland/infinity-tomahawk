using Aspire.Dashboard;
using Aspire.Dashboard.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infinity.Toolkit.FeatureModule.AspireDashboard;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAspireDashboard(this IServiceCollection services)
    {
        services.AddHostedService((sp) =>
        {
            var application = new DashboardWebApplication(sp.GetRequiredService<ILogger<DashboardWebApplication>>(), configureServices =>
            {
                configureServices.AddSingleton<IDashboardViewModelService, AspireDashboardViewModelService>();
            });

            return application;
        });

        return services;
    }
}
