using System;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.ProjectModel;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions
{
    public interface IComponentRegistry
    {
        IServiceProvider ServiceProvider { get; }
        void DiscoverAndRegisterComponents(IProjectContext projectContext);
        void RegisterComponent<T>(object instance) where T : class;
        void RegisterComponentWithDependencies<TService, TImplementation>() where TService : class;
        T OfType<T>() where T : class;
    }
}
