using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.FileSystem;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating;
using Microsoft.VisualStudio.Web.CodeGeneration.Templating.Razor.Compilation;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Templating.Razor
{
    public class RazorTemplateProcessor : TemplateProcessorBase
    {
        private ICompilationService _compilationService;

        public override string ProcessorId => "RAZOR";

        public RazorTemplateProcessor(IFileSystem fileSystem, ICompilationService compilationService)
            : base(fileSystem)
        {
            _compilationService = compilationService ?? throw new ArgumentNullException(nameof(compilationService));
        }


        protected override async Task<TemplateResult> ProcessSingleTemplate(string content,
            dynamic templateModel)
        {
            var razorEngine = RazorEngine.Create((builder) =>
            {
                RazorExtensions.Register(builder);
            });

            // Don't care about the RazorProject as we already have the content of the .cshtml file 
            // and don't need to deal with imports.
            var razorProject = RazorProject.Create(Directory.GetCurrentDirectory());
            var razorTemplateEngine = new RazorTemplateEngine(razorEngine, razorProject);

            var imports = new RazorSourceDocument[]
            {
                RazorSourceDocument.Create(@"
@using System
@using System.Threading.Tasks
", fileName: null)
            };

            var razorDocument = RazorCodeDocument.Create(RazorSourceDocument.Create(content, "Template"), imports);
            var generatorResults = razorTemplateEngine.GenerateCode(razorDocument);

            if (generatorResults.Diagnostics.Any())
            {
                var messages = generatorResults.Diagnostics.Select(d => d.GetMessage());
                return new TemplateResult()
                {
                    GeneratedText = string.Empty,
                    ProcessingException = new TemplateProcessingException(messages, generatorResults.GeneratedCode)
                };
            }
            var templateResult = _compilationService.Compile(generatorResults.GeneratedCode);
            if (templateResult.Messages.Any())
            {
                return new TemplateResult()
                {
                    GeneratedText = string.Empty,
                    ProcessingException = new TemplateProcessingException(templateResult.Messages, generatorResults.GeneratedCode)
                };
            }

            var compiledObject = Activator.CreateInstance(templateResult.CompiledType);
            var razorTemplate = compiledObject as RazorTemplateBase;

            string result = String.Empty;
            if (razorTemplate != null)
            {
                razorTemplate.Model = templateModel;
                //ToDo: If there are errors executing the code, they are missed here.
                result = await razorTemplate.ExecuteTemplate();
            }

            return new TemplateResult()
            {
                GeneratedText = result,
                ProcessingException = null
            };

        }
    }
}
