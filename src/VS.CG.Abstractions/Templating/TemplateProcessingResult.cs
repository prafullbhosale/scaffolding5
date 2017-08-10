// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating
{
    public class TemplateProcessingResult
    {
        public TemplateProcessingResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
            _generatedContent = new Dictionary<string, string>();
            _templateProcessingErrors = new Dictionary<string, string[]>();
        }

        public bool IsSuccess { get; set; }

        private Dictionary<string, string> _generatedContent;
        public Dictionary<string, string> GeneratedContent
        {
            get
            {
                return _generatedContent;
            }
        }

        private Dictionary<string, string[]> _templateProcessingErrors;

        public Dictionary<string, string[]> TemplateProcessingErrors
        {
            get
            {
                return _templateProcessingErrors;
            }
        }


        public void AddGeneratedContent(string sourcePath, string generatedText)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException(nameof(sourcePath));
            }

            _generatedContent[sourcePath] = generatedText;
        }

        public void AddGenerationErrors(string sourcePath, string[] errors)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException(nameof(sourcePath));
            }

            if (errors == null || errors.Length == 0)
            {
                errors = new[] { "Unknown error" };
            }

            _templateProcessingErrors[sourcePath] = errors;
        }

    }
}
