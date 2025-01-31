using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;

namespace Infinity.Toolkit.Azure.FeatureManagement;

/// <summary>
/// A feature filter that can be used to to activate a feature based on the current host environment.
/// </summary>
[FilterAlias(FilterAlias)]
public class EnvironmentFilter(IHostEnvironment environment) : IFeatureFilter
{
    private const string FilterAlias = "Infinity.Toolkit.EnvironmentFilter";
    private readonly IHostEnvironment env = environment ?? throw new ArgumentNullException(nameof(environment));

    public string[]? AllowedEnvironments { get; set; }

    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        var settings = context.Parameters.Get<EnvironmentFilterSettings>() ?? new EnvironmentFilterSettings();

        return Task.FromResult(settings.DisabledEnvironments.Any(t => t.Equals(env.EnvironmentName)));
    }
}
