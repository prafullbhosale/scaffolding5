using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.FileSystem;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.ProjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating;
using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Core.Templating
{
    public class TemplateLocator : ITemplateLocator
    {
        private const string RootTemplateFolderName = "Templates";
        private const string TemplateConfigFileName = ".template.config";
        private List<string> _searchPaths;
        private IFileSystem _fileSystem;
        private ILogger _logger;

        public TemplateLocator(IProjectContext projectContext, IFileSystem fileSystem, ILogger logger)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (projectContext == null)
            {
                throw new ArgumentNullException(nameof(projectContext));
            }

            _searchPaths = new List<string>();
            // The first place we want to look is the project we are scaffolding into.
            var projectOverridePath = Path.Combine(Path.GetDirectoryName(projectContext.ProjectFullPath), RootTemplateFolderName);
            if (Directory.Exists(projectOverridePath))
            {
                _searchPaths.Add(projectOverridePath);
            }

            if (projectContext.ProjectReferences != null)
            {
                // Look to see if there are templates/ overrides in project references.
                foreach (var project in projectContext.ProjectReferences)
                {
                    var candidatePath = Path.Combine(Path.GetDirectoryName(project), RootTemplateFolderName);
                    if (Directory.Exists(candidatePath))
                    {
                        _searchPaths.Add(candidatePath);
                    }
                }
            }

            if (projectContext.PackageDependencies != null)
            {
                // Look into NuGet packages for templates.
                foreach (var package in projectContext.PackageDependencies)
                {
                    var candidatePath = Path.Combine(package.Path, RootTemplateFolderName);
                    if (Directory.Exists(candidatePath))
                    {
                        _searchPaths.Add(candidatePath);
                    }
                }
            }
        }


        public ITemplate FindTemplate(string templateId)
        {
            var templateContentPath = "";
            foreach (var path in _searchPaths)
            {
                var candidatePath = Path.Combine(path, templateId);
                if (Directory.Exists(candidatePath)
                    && File.Exists(Path.Combine(candidatePath, TemplateConfigFileName)))
                {
                    templateContentPath = candidatePath;
                    break;
                }
            }

            if (string.IsNullOrEmpty(templateContentPath))
            {
                var msg = string.Format(MessageStrings.TemplateNotFoundError, templateId);
                _logger.Log(msg, LogMessageLevel.Trace);
                _logger.Log($"Search paths: {string.Join(Environment.NewLine, _searchPaths)}", LogMessageLevel.Trace);
                throw new InvalidOperationException(msg);
            }


            var templateConfigTxt = _fileSystem.ReadAllText(Path.Combine(templateContentPath, TemplateConfigFileName));
            var templateConfig = JsonConvert.DeserializeObject<ITemplate>(templateConfigTxt);

            if (templateConfig.CopyOnlySources != null)
            {
                for (int i = 0; i < templateConfig.CopyOnlySources.Length; i++)
                {
                    templateConfig.CopyOnlySources[i] = Path.Combine(templateContentPath, templateConfig.CopyOnlySources[i]);
                }
            }


            if (templateConfig.TemplateSources != null)
            {
                for (int i = 0; i < templateConfig.TemplateSources.Length; i++)
                {
                    templateConfig.TemplateSources[i] = Path.Combine(templateContentPath, templateConfig.TemplateSources[i]);
                }
            }

            return templateConfig;
        }
    }
}
