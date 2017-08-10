using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.ProjectModel;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Core
{
    public static class ProjectContextExtensions
    {
        public static DependencyDescription GetPackage(this IProjectContext context, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.PackageDependencies.FirstOrDefault(package => package.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<DependencyDescription> GetReferencingPackages(this IProjectContext context, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context
                .PackageDependencies
                .Where(package => package
                    .Dependencies
                    .Any(dep => dep.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
