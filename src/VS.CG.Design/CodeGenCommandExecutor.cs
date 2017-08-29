using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.FileSystem;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.ProjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating;
using Microsoft.VisualStudio.Web.CodeGeneration.Core;
using Microsoft.VisualStudio.Web.CodeGeneration.Core.Templating;
using Microsoft.VisualStudio.Web.CodeGeneration.Core.Workspace;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Design
{
    internal class CodeGenCommandExecutor
    {
        private readonly IProjectContext _projectInformation;
        private string[] _codeGenArguments;
        private string _configuration;
        private ILogger _logger;
        private bool _isSimulationMode;
        private CodeGenerationContext _codeGenerationContext;

        public CodeGenCommandExecutor(IProjectContext projectInformation, string[] codeGenArguments, string configuration, ILogger logger, bool isSimulationMode)
        {
            _projectInformation = projectInformation ?? throw new ArgumentNullException(nameof(projectInformation));
            _codeGenArguments = codeGenArguments ?? throw new ArgumentNullException(nameof(codeGenArguments));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration;
            _isSimulationMode = isSimulationMode;
        }


        internal async Task<int> Execute(Action<IEnumerable<FileSystemChangeInformation>> simModeAction = null)
        {
            var loadContext = new DefaultAssemblyLoadContext();
            var codeGenAssemblyProvider = new CodeGenerationAssemblyProvider(_projectInformation, loadContext);

            var componentRegistry = new ComponentRegistry(codeGenAssemblyProvider, _logger);
            AddFrameworkServices(componentRegistry, _projectInformation);

            AddServicesFromComponents(componentRegistry);

            var codeGenCommand = componentRegistry.OfType<CodeGenCommand>();
            var exitCode = await codeGenCommand.Execute(_codeGenerationContext, _codeGenArguments[0]);

            return 0;
        }

        private void AddFrameworkServices(ComponentRegistry componentRegistry, IProjectContext projectInformation)
        {
            //Ordering of services is important here

            var applicationInfo = new ApplicationInfo(
                projectInformation.ProjectName,
                Path.GetDirectoryName(projectInformation.ProjectFullPath));
            componentRegistry.RegisterComponent<IProjectContext>(projectInformation);

            _codeGenerationContext = new CodeGenerationContext(_codeGenArguments, projectInformation.ProjectFullPath);
            componentRegistry.RegisterComponent<CodeGenerationContext>(_codeGenerationContext);

            componentRegistry.RegisterComponent<IApplicationInfo>(applicationInfo);
            componentRegistry.RegisterComponent<ICodeGenAssemblyLoadContext>(new DefaultAssemblyLoadContext());
            componentRegistry.RegisterComponent<WorkspaceManager>(WorkspaceManager.Create(projectInformation));

            componentRegistry.RegisterComponent<IFileSystem>(DefaultFileSystem.Instance);
            componentRegistry.RegisterComponent<ILogger>(_logger);

            componentRegistry.RegisterComponentWithDependencies<ICodeGenerationAssemblyProvider, CodeGenerationAssemblyProvider>();
            componentRegistry.RegisterComponentWithDependencies<ITemplateLocator, TemplateLocator>();

            componentRegistry.RegisterComponentWithDependencies<CodeGenCommand, CodeGenCommand>();
        }

        private void AddServicesFromComponents(ComponentRegistry componentRegistry)
        {
            componentRegistry.DiscoverAndRegisterComponents(_projectInformation);
        }
    }
}