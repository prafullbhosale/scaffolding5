// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.ProjectModel;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Core.Workspace
{
    public class WorkspaceManager
    {
        private CodeAnalysis.Solution _solution;
        private readonly string _rootProjectPath;

        private object _syncObject = new object();

        public static WorkspaceManager Create(IProjectContext projectInformation)
        {
            return new WorkspaceManager(new RoslynWorkspace(projectInformation), projectInformation.ProjectFullPath);
        }

        private WorkspaceManager(RoslynWorkspace workSpace, string rootProjectPath)
        {
            if (workSpace == null)
            {
                throw new ArgumentNullException(nameof(workSpace));
            }

            if (string.IsNullOrEmpty(rootProjectPath))
            {
                throw new ArgumentException(nameof(rootProjectPath));
            }

            _solution = workSpace.CurrentSolution;
            _rootProjectPath = rootProjectPath;
        }

        public Task<Compilation> GetCompilationAsync() => GetCompilationAsync(_rootProjectPath);

        public Task<Compilation> GetCompilationAsync(string projectPath)
        {
            if (string.IsNullOrEmpty(projectPath))
            {
                throw new ArgumentException(nameof(projectPath));
            }

            lock (_syncObject)
            {
                var project = _solution
                    .Projects
                    .FirstOrDefault(p => p.FilePath.Equals(projectPath, StringComparison.OrdinalIgnoreCase));

                if (project == null)
                {
                    throw new InvalidOperationException(string.Format("Project not in the current workspace: '{0}'", projectPath));
                }

                return project.GetCompilationAsync();
            }
        }

        public void AddDocumentToProject(string projectPath, string documentPath, string content, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(projectPath))
            {
                throw new ArgumentException(nameof(projectPath));
            }

            lock (_syncObject)
            {
                var project = _solution
                    .Projects
                    .FirstOrDefault(p => p.FilePath.Equals(projectPath, StringComparison.OrdinalIgnoreCase));

                if (project == null)
                {
                    throw new InvalidOperationException(string.Format("Project not in the current workspace: '{0}'", projectPath));
                }

                if (!Path.IsPathRooted(documentPath))
                {
                    documentPath = Path.Combine(Path.GetDirectoryName(projectPath), documentPath);
                }

                AddDocumentInternal(documentPath, content, encoding, project);
            }
        }

        private void AddDocumentInternal(string documentPath, string content, Encoding encoding, Project project)
        {
            var sourceText = SourceText.From(content, encoding ?? Encoding.UTF8);
            var doc = project.AddDocument(documentPath, sourceText);

            // Get the new Workspace.
            _solution = doc.Project.Solution;
        }

        public void EditDocumentInProject(string projectPath, string documentPath, string content, Encoding encoding = null)
        {
            lock (_syncObject)
            {
                if (string.IsNullOrEmpty(projectPath))
                {
                    throw new ArgumentException(nameof(projectPath));
                }

                var project = _solution
                    .Projects
                    .FirstOrDefault(p => p.FilePath.Equals(projectPath, StringComparison.OrdinalIgnoreCase));

                if (project == null)
                {
                    throw new InvalidOperationException(string.Format("Project not in the current workspace: '{0}'", projectPath));
                }

                if (!Path.IsPathRooted(documentPath))
                {
                    documentPath = Path.Combine(Path.GetDirectoryName(projectPath), documentPath);
                }

                var document = project.Documents.FirstOrDefault(d => d.FilePath.Equals(documentPath));

                if (document != null)
                {
                    project = project.RemoveDocument(document.Id);
                }

                AddDocumentInternal(documentPath, content, encoding, project);
            }
        }
    }
}
