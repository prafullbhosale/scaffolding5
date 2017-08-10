using System.Reflection;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Core
{
    public interface ICodeGenAssemblyLoadContext
    {
        Assembly LoadFromName(AssemblyName assemblyName);
    }
}