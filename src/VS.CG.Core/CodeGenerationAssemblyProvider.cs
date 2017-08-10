using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.ProjectModel;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Core
{
    public class CodeGenerationAssemblyProvider : ICodeGenerationAssemblyProvider
    {
        private static readonly HashSet<string> _codeGenerationFrameworkAssemblies =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "Microsoft.VisualStudio.Web.CodeGeneration",
            };
        private static readonly HashSet<string> _exclusions =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "Microsoft.VisualStudio.Web.CodeGeneration.Tools",
                "Microsoft.VisualStudio.Web.CodeGeneration",
                "Microsoft.VisualStudio.Web.CodeGeneration.Design"
            };

        private readonly ICodeGenAssemblyLoadContext _assemblyLoadContext;
        private IProjectContext _projectContext;

        public CodeGenerationAssemblyProvider(IProjectContext projectContext, ICodeGenAssemblyLoadContext loadContext)
        {
            if (loadContext == null)
            {
                throw new ArgumentNullException(nameof(loadContext));
            }
            if (projectContext == null)
            {
                throw new ArgumentNullException(nameof(projectContext));
            }
            _projectContext = projectContext;
            _assemblyLoadContext = loadContext;

        }

        public IEnumerable<Assembly> CandidateAssemblies
        {
            get
            {

                var list = _codeGenerationFrameworkAssemblies
                    .SelectMany(_projectContext.GetReferencingPackages)
                    .Distinct()
                    .Where(IsCandidateLibrary);
                return list.Select(lib => _assemblyLoadContext.LoadFromName(new AssemblyName(lib.Name)));
            }
        }

        private bool IsCandidateLibrary(DependencyDescription library)
        {
            return !_exclusions.Contains(library.Name);
        }
    }
}
