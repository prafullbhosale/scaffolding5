using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Templating.Razor.Compilation
{
    public class AssemblyGenerationResult
    {
        public bool Success { get; set; }

        public Assembly Assembly { get; set; }

        public IEnumerable<string> ErrorMessages { get; set; }

        public static AssemblyGenerationResult FromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return new AssemblyGenerationResult()
            {
                Assembly = assembly,
                Success = true
            };
        }

        public static AssemblyGenerationResult FromErrorMessages(IEnumerable<string> errorMessages)
        {
            if (errorMessages == null)
            {
                throw new ArgumentNullException(nameof(errorMessages));
            }

            return new AssemblyGenerationResult()
            {
                ErrorMessages = errorMessages,
                Success = false
            };
        }
    }
}