// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.ProjectModel
{
    /// <summary>
    /// Information about a resolved reference of the project.
    /// The reference could be from a NuGet package, assembly ref etc.
    /// </summary>
    public class ResolvedReference
    {
        /// <summary/>
        /// <param name="name">Name of the referenced assembly.</param>
        /// <param name="resolvedPath">Full path of the referenced assembly.</param>
        public ResolvedReference(string name, string resolvedPath)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            if (string.IsNullOrEmpty(resolvedPath))
            {
                throw new ArgumentException(nameof(resolvedPath));
            }

            Name = name;
            ResolvedPath = resolvedPath;
        }

        /// <summary>
        /// Full path of the referenced assembly.
        /// </summary>
        public string ResolvedPath { get; }

        /// <summary>
        /// Name of the referenced assembly.
        /// </summary>
        public string Name { get; }
    }
}
