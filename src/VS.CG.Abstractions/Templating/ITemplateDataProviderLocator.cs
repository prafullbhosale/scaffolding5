using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating
{
    public interface ITemplateDataProviderLocator
    {
        IDictionary<string, ITemplateDataProvider> FindDataProvider(ITemplate template);
    }
}
