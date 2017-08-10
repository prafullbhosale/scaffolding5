// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Logging
{
    public interface ILogger
    {
        void Log(string message, LogMessageLevel logMessageLevel = LogMessageLevel.Information);
        void LogError(string errorMessage);
        void LogException(Exception ex);

    }

    public enum LogMessageLevel
    {
        Information,
        Error,
        Trace
    }
}
