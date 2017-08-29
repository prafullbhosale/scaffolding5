using System;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating;
using Microsoft.VisualStudio.Web.CodeGeneration.Templating.Razor;
using Microsoft.VisualStudio.Web.CodeGeneration.Templating.Razor.Compilation;

namespace Microsoft.VisualStudio.Web.CodeGenerators.MVC
{
    public class AspNetScaffolderComponentProvider : IComponentsProvider
    {
        public void RegisterComponents(IComponentRegistry componentRegistry)
        {
            if (componentRegistry == null)
            {
                throw new ArgumentNullException(nameof(componentRegistry));
            }

            componentRegistry.RegisterComponent<ICompilationService>(new RoslynCompilationService());

            componentRegistry.RegisterComponentWithDependencies<ITemplateProcessor, RazorTemplateProcessor>();
        }
    }
}
