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

        public ComponentRegistry(CodeGenerationAssemblyProvider assemblyProvider, ILogger logger)
        {
            _codeGenAssemblyProvider = assemblyProvider ?? throw new ArgumentNullException(nameof(assemblyProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void DiscoverAndRegisterComponents(IProjectContext projectContext, IServiceProvider serviceProvider)
        {
            if (projectContext == null)
            {
                throw new ArgumentNullException(nameof(projectContext));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
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
                        instance.RegisterComponents(serviceProvider);
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
    }
}
