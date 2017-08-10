using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Core
{
    public class DataProviderDescriptor
    {
        private readonly TypeInfo _dataProviderType;
        private readonly IServiceProvider _serviceProvider;

        public DataProviderDescriptor(TypeInfo dataProviderType,
            IServiceProvider serviceProvider)
        {
            _dataProviderType = dataProviderType ?? throw new ArgumentNullException(nameof(dataProviderType));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
    }
}
