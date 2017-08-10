using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.ProjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Engine
{
    public class CodeGenerationEngine
    {
        private ITemplateProcessor _templateProcessor;
        private ICodeGenerationSettings _settings;
        private IProjectContext _projectContext;

        public CodeGenerationEngine(ITemplateProcessor templateProcessor, ICodeGenerationSettings settings, IProjectContext projectContext)
        {
            _templateProcessor = templateProcessor ?? throw new ArgumentNullException(nameof(templateProcessor));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _projectContext = projectContext ?? throw new ArgumentNullException(nameof(projectContext));
        }

        Stack<CodeGenerationTask> _codeGenerationTasks;

        public Task GenerateAsync(CodeGenerationContext context)
        {
            BuildTaskSpec(context);
            return Task.CompletedTask;
        }

        private void BuildTaskSpec(CodeGenerationContext context)
        {
            _codeGenerationTasks = new Stack<CodeGenerationTask>();

        }
    }
}
