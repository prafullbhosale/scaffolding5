using System;
using System.Linq;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.ProjectModel;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Core
{
    public class ComponentRegistry : IComponentRegistry
    {
        private CodeGenerationAssemblyProvider _codeGenAssemblyProvider;
        private ILogger _logger;
        private ServiceProvider _serviceProvider;

        public ComponentRegistry(CodeGenerationAssemblyProvider assemblyProvider, ILogger logger)
        {
            _codeGenAssemblyProvider = assemblyProvider ?? throw new ArgumentNullException(nameof(assemblyProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = new ServiceProvider();
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

        public void DiscoverAndRegisterComponents(IProjectContext projectContext)
        {
            if (projectContext == null)
            {
                throw new ArgumentNullException(nameof(projectContext));
            }

            var candidates = _codeGenAssemblyProvider
                .CandidateAssemblies
                .SelectMany(asm => asm.ExportedTypes)
                .Where(t => typeof(IComponentsProvider).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);

            if (candidates != null)
            {
                foreach (var candidate in candidates)
                {
                    try
                    {
                        var instance = Activator.CreateInstance(candidate) as IComponentsProvider;
                        instance.RegisterComponents(this);

                        _logger.Log($"Registered components by: {candidate.AssemblyQualifiedName}", LogMessageLevel.Trace);
                    }
                    catch (Exception ex)
                    {
                        // We may not need all components for the current operation.
                        _logger.Log($"Failed to register components from type: {candidate.AssemblyQualifiedName}");
                        _logger.Log(ex.Message, LogMessageLevel.Trace);
                        _logger.Log(ex.StackTrace, LogMessageLevel.Trace);
                        continue;
                    }
                }
            }
            else
            {
                _logger.Log("No components provider found.", LogMessageLevel.Trace);
            }
        }

        public T OfType<T>() where T : class
        {
            var candidate = _serviceProvider.GetService(typeof(T));
            if (candidate is T)
            {
                return candidate as T;
            }

            throw new InvalidOperationException($"Could not get component of type {typeof(T).FullName}");
        }

        public void RegisterComponent<T>(object instance) where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            _serviceProvider.Add<T>(instance);
        }

        public void RegisterComponentWithDependencies<TService, TImplementation>() where TService : class
        {
            _serviceProvider.AddServiceWithDependencies<TService, TImplementation>();
        }
    }
}
