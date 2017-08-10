namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating
{
    public interface ITemplateLocator
    {
        // TODO: templateId?
        ITemplate FindTemplate(string templateId);
    }
}
