using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Core
{
    public interface ICodeGenerationAssemblyProvider
    {
        IEnumerable<Assembly> CandidateAssemblies { get; }
    }
}
