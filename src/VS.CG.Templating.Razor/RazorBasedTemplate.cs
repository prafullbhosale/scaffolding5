using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Templating.Razor
{
    public class RazorBasedTemplate : TemplateBase
    {
        public override sealed string Processor => "Razor";
    }
}
