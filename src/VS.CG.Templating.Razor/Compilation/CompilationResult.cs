using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Templating.Razor.Compilation
{
    public class CompilationResult
    {
        private readonly Type _type;

        private CompilationResult(string sourceCode, Type type, IEnumerable<string> messages)
        {
            _type = type;
            SourceCode = sourceCode;
            Messages = messages;
        }

        public IEnumerable<string> Messages { get; private set; }

        public string SourceCode { get; private set; }

        public Type CompiledType
        {
            get
            {
                if (_type == null)
                {
                    throw new TemplateProcessingException(Messages, SourceCode);
                }

                return _type;
            }
        }

        public static CompilationResult Failed(string sourceCode, IEnumerable<string> messages)
        {
            return new CompilationResult(sourceCode, type: null, messages: messages);
        }

        public static CompilationResult Successful(string sourceCode, Type type)
        {
            return new CompilationResult(sourceCode, type, Enumerable.Empty<string>());
        }
    }
}
