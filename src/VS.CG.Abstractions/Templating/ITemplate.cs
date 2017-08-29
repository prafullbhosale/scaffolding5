// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating
{
    public interface ITemplate
    {
        string [] TemplateSources { get; }
        string[] CopyOnlySources { get; }

        string[] DataContracts { get; }
    }

    public abstract class TemplateBase : ITemplate
    {
        public string[] TemplateSources { get; set; }
        public string[] CopyOnlySources { get; set; }

        public string[] DataContracts { get; set; }

        public abstract string Processor { get; }
    }
}
