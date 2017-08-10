// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Messaging
{
    public class MessageType
    {
        public MessageType(string value, int minProtocolVersion)
        {
            Value = value;
            MinProtocolVersion = minProtocolVersion;
        }

        public string Value { get; }

        public int MinProtocolVersion { get; }
    }
}
