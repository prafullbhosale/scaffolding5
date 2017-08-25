using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Design
{
    internal class CodeGenCommand
    {
        private ILogger _logger;
        private ITemplateLocator _templateLocator;
        private ITemplateDataProviderLocator _dataProviderLocator;
        private ITemplateProcessor _templateProcessor;

        public CodeGenCommand(ILogger logger,
            ITemplateLocator templateLocator,
            ITemplateDataProviderLocator dataProviderLocator,
            ITemplateProcessor templateProcessor)

        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _templateLocator = templateLocator ?? throw new ArgumentNullException(nameof(templateLocator));
            _dataProviderLocator = dataProviderLocator ?? throw new ArgumentNullException(nameof(dataProviderLocator));
            _templateProcessor = templateProcessor ?? throw new ArgumentNullException(nameof(templateProcessor));
        }

        public async Task<int> Execute(CodeGenerationContext codeGenerationContext, string codeGeneratorName)
        {
            if (codeGenerationContext == null)
            {
                throw new ArgumentNullException(nameof(codeGenerationContext));
            }

            var template = _templateLocator.FindTemplate(codeGeneratorName) as TemplateBase;
            var dataProvider = _dataProviderLocator.FindDataProvider(template);

            var templateData = dataProvider.GetData(template.DataContract, codeGenerationContext);

            var templateProcessingResult = await _templateProcessor.ProcessTemplateAsync(template, templateData);

            if (templateProcessingResult.IsSuccess)
            {
                // TODO: Handle generated content.
                var generatedContent = templateProcessingResult.GeneratedContent;
            }
            else
            {
                // TODO: Handle errors.
                var errors = templateProcessingResult.TemplateProcessingErrors;
            }


            return 0;
        }
    }
}