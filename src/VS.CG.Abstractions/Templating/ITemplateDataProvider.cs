// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating
{
    public interface ITemplateDataProvider
    {
        string[] DataContracts { get; }
        JObject GetData(string dataContract, CodeGenerationContext context);
    }

    public abstract class TemplateDataProviderBase : ITemplateDataProvider
    {
        public TemplateDataProviderBase(string [] dataContracts)
        {
            if (dataContracts == null)
            {
                throw new ArgumentNullException(nameof(dataContracts));
            }

            if (!dataContracts.Any())
            {
                throw new InvalidOperationException("Cannot create a dataprovider without any data contracts.");
            }

            DataContracts = dataContracts;
        }

        public string[] DataContracts { get; }

        public JObject GetData(string dataContract, CodeGenerationContext context)
        {
            if (string.IsNullOrEmpty(nameof(dataContract)))
            {
                throw new ArgumentException(nameof(dataContract));
            }

            if (!DataContracts.Any(dc => dc.Equals(dataContract, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Data Contract not supported: '{dataContract}'");
            }

            return GetDataForContract(dataContract, context);
        }

        protected abstract JObject GetDataForContract(string dataContract, CodeGenerationContext context);
    }
}
