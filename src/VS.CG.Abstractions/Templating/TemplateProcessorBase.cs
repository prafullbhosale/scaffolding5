using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.FileSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating
{
    public abstract class TemplateProcessorBase : ITemplateProcessor
    {
        private IFileSystem _fileSystem;

        public TemplateProcessorBase(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public abstract string ProcessorId { get; }

        protected IFileSystem FileSystem
        {
            get
            {
                return _fileSystem;

            }
        }

        public virtual async Task<TemplateProcessingResult> ProcessTemplateAsync(TemplateBase template, JToken templateData)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            ValidateTemplateForProcessor(template);

            var result = ProcessCopyOnlyFiles(template);

            dynamic templateModel = null;
            if (templateData != null)
            {
                templateModel = JsonConvert.DeserializeObject(templateData.ToString());
            }

            foreach (var templateSource in template.TemplateSources)
            {
                try
                {
                    var templateContent = FileSystem.ReadAllText(templateSource);
                    var templateResult = await ProcessSingleTemplate(templateContent, templateModel);
                    if (templateResult.TemplateProcessingException != null)
                    {
                        result.IsSuccess = false;
                        result.TemplateProcessingErrors[templateSource] = templateResult.TemplateProcessingException.Messages.ToArray();
                    }
                    else
                    {
                        result.GeneratedContent[templateSource] = templateResult.GeneratedContent;
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.TemplateProcessingErrors[templateSource] = new[] { ex.Message };
                }
            }

            return result;
        }

        private void ValidateTemplateForProcessor(TemplateBase template)
        {
            if (template.Processor.Equals(ProcessorId, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(string.Format(MessageStrings.InvalidTemplateForProcessorError, ProcessorId));
            }
        }

        protected virtual TemplateProcessingResult ProcessCopyOnlyFiles(TemplateBase template, TemplateProcessingResult result = null)
        {
            if (result == null)
            {
                result = new TemplateProcessingResult(true);
            }

            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (template.CopyOnlySources != null)
            {
                foreach (var source in template.CopyOnlySources)
                {
                    try
                    {
                        var content = FileSystem.ReadAllText(source);
                        result.GeneratedContent[source] = content;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.TemplateProcessingErrors[source] = new[] { ex.Message };
                    }
                }
            }

            return result;
        }

        protected abstract Task<TemplateResult> ProcessSingleTemplate(string content, dynamic templateModel);
    }
}
