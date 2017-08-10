namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating
{
    public class TemplateResult
    {
        public string GeneratedText { get; set; }
        public TemplateProcessingException ProcessingException { get; set; }
    }
}