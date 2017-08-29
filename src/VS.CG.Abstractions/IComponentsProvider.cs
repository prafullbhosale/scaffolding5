using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions
{
    public interface IComponentsProvider
    {
        void RegisterComponents(IComponentRegistry componentRegistry);
    }
}