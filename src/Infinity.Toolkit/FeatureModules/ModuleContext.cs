using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infinity.Toolkit.FeatureModules;

public sealed class ModuleContext
{
    /// <summary>
    /// Gets the host environment.
    /// </summary>
    public IHostEnvironment Environment { get; init; }

    /// <summary>
    /// Gets the service collection.
    /// </summary>
    public IServiceCollection Services { get; init; }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public IConfiguration Configuration { get; init; }
}
