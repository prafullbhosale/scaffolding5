namespace Microsoft.VisualStudio.Web.CodeGeneration.Core
{
    public interface IApplicationInfo
    {
        string ApplicationBasePath { get; }
        string ApplicationName { get; }
        string ApplicationConfiguration { get; }
    }
}