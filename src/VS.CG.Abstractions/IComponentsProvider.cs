using System;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions
{
    public interface IComponentsProvider
    {
        void RegisterComponents(IServiceProvider serviceProvider);
    }
}