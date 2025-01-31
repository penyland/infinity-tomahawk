using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;

namespace Infinity.Toolkit.Azure.FeatureManagement;

public static class Extensions
{
    internal const string DefaultConfigSectionName = "Infinity:FeatureManagement";

    public static IFeatureManagementBuilder AddFeatureManagement(this IHostApplicationBuilder builder)
    {
        // Disabling endpoints in specific environments.
        //
        // By using the feature filter EnvironmentFilter you can enable or disable endpoints and/or actions depending on the current execution environment.
        // You do that by decorating the controller/action with the FeatureGate attribute and adding configuration for the FeatureFilter in either Azure App Configuration (the preferred option) or appsettings.json
        // Example: [FeatureGate(FeatureFlags.NameOfYourFeatureFlag)]
        //
        // In Azure App Configuration create a new Feature flag with the key ApplicationName:NameOfYourFeatureFlag, also enable "Use Feature filter" and select custom and set the name to EnvironmentFilter. Press Apply.
        // Select the three dots to the right of you feature flag and select Advanced Edit. Under parameters property add the json below:
        //
        // "AllowedEnvironments": [
        //   "LocalDevelopment",
        //   "Development"
        // ]
        //
        // It should look like this:
        // "parameters": {
        //   "AllowedEnvironments": [
        //     "LocalDevelopment",
        //     "Development",
        //     "Test",
        //     "Reference"
        //   ]
        // }
        //
        // In appsettings.json (only when developing locally)
        //
        // "Infinity:FeatureManagement": {
        //   "NameOfYourFeatureFlag": {
        //     "EnabledFor": [
        //       {
        //         "Name": "EnvironmentFilter",
        //         "Parameters": {
        //           "AllowedEnvironments": [
        //           "LocalDevelopment",
        //           "Development",
        //           "Test"
        //         ]
        //       }
        //     }
        //   ]
        // }
        return builder.Services.AddFeatureManagement(builder.Configuration.GetSection(DefaultConfigSectionName))
            .AddFeatureFilter<EnvironmentFilter>();
    }
}
