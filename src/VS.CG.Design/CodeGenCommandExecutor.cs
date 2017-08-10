using System;
using System.Collections.Generic;
using System.IO;
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

        public CodeGenCommandExecutor(IProjectContext projectInformation, string[] codeGenArguments, string configuration, ILogger logger, bool isSimulationMode)
        {
            _projectInformation = projectInformation ?? throw new ArgumentNullException(nameof(projectInformation));
            _codeGenArguments = codeGenArguments ?? throw new ArgumentNullException(nameof(codeGenArguments));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration;
            _isSimulationMode = isSimulationMode;
        }


        internal int Execute(Action<IEnumerable<FileSystemChangeInformation>> simModeAction = null)
        {
            var serviceProvider = new ServiceProvider();
            AddFrameworkServices(serviceProvider, _projectInformation);
            AddCodeGenerationServices(serviceProvider);
            return 0;
        }

        private void AddFrameworkServices(ServiceProvider serviceProvider, IProjectContext projectInformation)
        {
            var applicationInfo = new ApplicationInfo(
                projectInformation.ProjectName,
                Path.GetDirectoryName(projectInformation.ProjectFullPath));
            serviceProvider.Add<IProjectContext>(projectInformation);
            serviceProvider.Add<IApplicationInfo>(applicationInfo);
            serviceProvider.Add<ICodeGenAssemblyLoadContext>(new DefaultAssemblyLoadContext());

            serviceProvider.Add<WorkspaceManager>(WorkspaceManager.Create(projectInformation));
            serviceProvider.Add<IFileSystem>(new DefaultFileSystem());
        }

        private void AddCodeGenerationServices(ServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            IFileSystem fileSystem = DefaultFileSystem.Instance;
            //Ordering of services is important here
            serviceProvider.Add(typeof(IFileSystem), fileSystem);
            serviceProvider.Add(typeof(ILogger), _logger);

            serviceProvider.AddServiceWithDependencies<ICodeGenerationAssemblyProvider, CodeGenerationAssemblyProvider>();
            serviceProvider.AddServiceWithDependencies<ITemplateLocator, TemplateLocator>();

            //serviceProvider.AddServiceWithDependencies<ICompilationService, RoslynCompilationService>();
            //serviceProvider.AddServiceWithDependencies<ITemplating, RazorTemplating>();

            //serviceProvider.AddServiceWithDependencies<IModelTypesLocator, ModelTypesLocator>();
            //serviceProvider.AddServiceWithDependencies<ICodeGeneratorActionsService, CodeGeneratorActionsService>();

            //serviceProvider.AddServiceWithDependencies<IDbContextEditorServices, DbContextEditorServices>();
            //serviceProvider.AddServiceWithDependencies<IEntityFrameworkService, EntityFrameworkServices>();
            //serviceProvider.AddServiceWithDependencies<ICodeModelService, CodeModelService>();
        }
    }
}