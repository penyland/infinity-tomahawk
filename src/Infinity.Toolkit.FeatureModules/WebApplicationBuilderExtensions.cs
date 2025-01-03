﻿namespace Infinity.Toolkit.FeatureModules;

public static class WebApplicationBuilderExtensions
{
    private const string FeatureModulesConfigKey = "FeatureModules";

    /// <summary>
    /// Add all feature modules that are found in the solution.
    /// </summary>
    public static WebApplicationBuilder AddFeatureModules(
        this WebApplicationBuilder builder,
        Action<FeatureModuleOptions> configure,
        string configKey = FeatureModulesConfigKey,
        ILoggerFactory? loggerFactory = null)
    {
        var options = new FeatureModuleOptions();
        builder.Configuration.GetSection(configKey).Bind(options);
        configure(options);

        return builder.RegisterFeatureModules(options, loggerFactory);
    }

    /// <summary>
    /// Add all feature modules that are found in the solution.
    /// </summary>
    public static WebApplicationBuilder AddFeatureModules(
        this WebApplicationBuilder builder,
        IConfiguration config)
    {
        var options = new FeatureModuleOptions();
        config.Bind(options);
        return builder.RegisterFeatureModules(options, null);
    }

    /// <summary>
    /// Add all feature modules that are found in the solution.
    /// </summary>
    public static WebApplicationBuilder AddFeatureModules(this WebApplicationBuilder builder)
    {
        return builder.AddFeatureModules(options => { });
    }

    internal static WebApplicationBuilder RegisterFeatureModules(this WebApplicationBuilder builder, FeatureModuleOptions options, ILoggerFactory? loggerFactory)
    {
        builder.Services.RegisterFeatureModules(builder.Configuration, builder.Environment, options, loggerFactory);
        return builder;
    }
}
