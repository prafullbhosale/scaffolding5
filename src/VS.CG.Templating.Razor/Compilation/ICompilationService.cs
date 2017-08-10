namespace Microsoft.VisualStudio.Web.CodeGeneration.Templating.Razor.Compilation
{
    public interface ICompilationService
    {
        CompilationResult Compile(string content);
    }
}