using Aspire.Dashboard.Model;
using Microsoft.AspNetCore.Hosting;

namespace Infinity.Toolkit.FeatureModule.AspireDashboard;

public record AspireDashboardViewModelService(string ApplicationName) : IDashboardViewModelService
{
    public AspireDashboardViewModelService(IWebHostEnvironment webHostEnvironment) : this(webHostEnvironment.ApplicationName)
    {
    }

    public ValueTask<List<ContainerViewModel>> GetContainersAsync() => ValueTask.FromResult(new List<ContainerViewModel>());

    public ValueTask<List<ExecutableViewModel>> GetExecutablesAsync() => ValueTask.FromResult(new List<ExecutableViewModel>());

    public ValueTask<List<ProjectViewModel>> GetProjectsAsync() => ValueTask.FromResult(new List<ProjectViewModel>());

    public IAsyncEnumerable<ResourceChanged<ContainerViewModel>> WatchContainersAsync(IEnumerable<NamespacedName>? existingContainers = null, CancellationToken cancellationToken = default) => AsyncEnumerable.Empty<ResourceChanged<ContainerViewModel>>();

    public IAsyncEnumerable<ResourceChanged<ExecutableViewModel>> WatchExecutablesAsync(IEnumerable<NamespacedName>? existingExecutables = null, CancellationToken cancellationToken = default) => AsyncEnumerable.Empty<ResourceChanged<ExecutableViewModel>>();

    public IAsyncEnumerable<ResourceChanged<ProjectViewModel>> WatchProjectsAsync(IEnumerable<NamespacedName>? existingProjects = null, CancellationToken cancellationToken = default) => AsyncEnumerable.Empty<ResourceChanged<ProjectViewModel>>();
}