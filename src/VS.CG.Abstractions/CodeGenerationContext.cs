// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions
{
    public class CodeGenerationContext
    {
        private ConcurrentDictionary<object, object> _items;

        public CodeGenerationContext(string[] args, string projectPath)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (string.IsNullOrEmpty(projectPath))
            {
                throw new ArgumentException(nameof(projectPath));
            }


            Args = args;
            ProjectPath = projectPath;

            _items = new ConcurrentDictionary<object, object>();

        }

        public string [] Args { get; }
        public string ProjectPath { get; }

        public T GetItem<T> (object key) where T: class
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            object value = null;
            if (_items.TryGetValue(key, out value) && value is T)
            {
                return value as T;
            }

            return null;
        }

        public void AddItem(object key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _items[key] = value;
        }
    }
}
