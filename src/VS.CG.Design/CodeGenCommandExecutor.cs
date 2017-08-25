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
            var serviceProvider = new ServiceProvider();
            AddFrameworkServices(serviceProvider, _projectInformation);
            
            AddCodeGenerationServices(serviceProvider);


            var codeGenCommand = serviceProvider.GetService(typeof(CodeGenCommand)) as CodeGenCommand;
            var exitCode = await codeGenCommand.Execute(_codeGenerationContext, _codeGenArguments[0]);

            return 0;
        }

        private void AddFrameworkServices(ServiceProvider serviceProvider, IProjectContext projectInformation)
        {
            //Ordering of services is important here

            var applicationInfo = new ApplicationInfo(
                projectInformation.ProjectName,
                Path.GetDirectoryName(projectInformation.ProjectFullPath));
            serviceProvider.Add<IProjectContext>(projectInformation);

            _codeGenerationContext = new CodeGenerationContext(_codeGenArguments, projectInformation.ProjectFullPath);
            serviceProvider.Add<CodeGenerationContext>(_codeGenerationContext);

            serviceProvider.Add<IApplicationInfo>(applicationInfo);
            serviceProvider.Add<ICodeGenAssemblyLoadContext>(new DefaultAssemblyLoadContext());
            serviceProvider.Add<WorkspaceManager>(WorkspaceManager.Create(projectInformation));

            serviceProvider.Add(typeof(IFileSystem), DefaultFileSystem.Instance);
            serviceProvider.Add(typeof(ILogger), _logger);

            serviceProvider.AddServiceWithDependencies<ICodeGenerationAssemblyProvider, CodeGenerationAssemblyProvider>();
            serviceProvider.AddServiceWithDependencies<ITemplateLocator, TemplateLocator>();

            serviceProvider.AddServiceWithDependencies<CodeGenCommand, CodeGenCommand>();
        }

        private void AddCodeGenerationServices(ServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            serviceProvider.AddServiceWithDependencies<IComponentRegistry, ComponentRegistry>();
            var componentRegistry = serviceProvider.GetService(typeof(IComponentRegistry)) as IComponentRegistry;

            componentRegistry.DiscoverAndRegisterComponents(_projectInformation, serviceProvider);
        }
    }
}