﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Core
{
    public class ApplicationInfo : IApplicationInfo
    {
        public ApplicationInfo(string appName, string appBasePath)
            : this(appName, appBasePath, "Debug")
        {

        }

        public ApplicationInfo(string appName, string appBasePath, string appConfiguration)
        {
            if (appName == null)
            {
                throw new ArgumentNullException(nameof(appName));
            }
            if (appBasePath == null)
            {
                throw new ArgumentNullException(nameof(appBasePath));
            }
            if (appConfiguration == null)
            {
                throw new ArgumentNullException(nameof(appConfiguration));
            }
            ApplicationName = appName;
            ApplicationBasePath = appBasePath;
            ApplicationConfiguration = appConfiguration;
        }
        public string ApplicationBasePath
        {
            get; private set;
        }

        public string ApplicationName
        {
            get; private set;
        }

        public string ApplicationConfiguration
        {
            get; private set;
        }
    }
}
