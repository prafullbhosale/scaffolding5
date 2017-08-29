using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating;
using Newtonsoft.Json.Linq;

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
            var templateData = GetTemplateData(codeGenerationContext, template);

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

        private JObject GetTemplateData(CodeGenerationContext codeGenerationContext, TemplateBase template)
        {
            var dataProviders = _dataProviderLocator.FindDataProvider(template);
            IEnumerable<string> missingDataProviders = null;
            if (dataProviders == null)
            {
                missingDataProviders = template.DataContracts;
            }
            else
            {
                missingDataProviders = template.DataContracts.Where(t => !dataProviders.ContainsKey(t));
            }

            if (missingDataProviders.Any())
            {
                throw new InvalidOperationException(
                    string.Format("Could not locate data providers for the following data contracts: {0}",
                        string.Join(',', missingDataProviders)));
            }

            var result = JObject.Parse("{}");
            foreach (var dp in dataProviders)
            {
                var dataObj = dp.Value.GetData(dp.Key, codeGenerationContext);
                if (dataObj == null)
                {
                    _logger.Log(string.Format("Received null data for data contract: {0}", dp.Key), LogMessageLevel.Trace);
                }
            }

            return result;
        }
    }
}