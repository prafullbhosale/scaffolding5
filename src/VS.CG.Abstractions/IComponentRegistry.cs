using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.ProjectModel;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions
{
    public interface IComponentRegistry
    {
        void DiscoverAndRegisterComponents(IProjectContext projectContext, IServiceProvider serviceProvider);
    }
}
