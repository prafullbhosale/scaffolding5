// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating
{
    public interface ITemplateProcessor
    {
        Task<TemplateProcessingResult> ProcessTemplateAsync(TemplateBase template, JObject templateData);
    }
}
