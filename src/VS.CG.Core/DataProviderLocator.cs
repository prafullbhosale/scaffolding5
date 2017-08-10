using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.ProjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Templating;
using Microsoft.VisualStudio.Web.CodeGeneration.Core;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Core
{
    public class DataProviderLocator
    {
        private IServiceProvider _serviceProvider;
        private IProjectContext _projectContext;
        private ICodeGenerationAssemblyProvider _codeGenerationAssemblyProvider;

        public DataProviderLocator(IProjectContext projectContext, ICodeGenerationAssemblyProvider codeGenerationAssemblyProvider, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _projectContext = projectContext ?? throw new ArgumentNullException(nameof(projectContext));
            _codeGenerationAssemblyProvider = codeGenerationAssemblyProvider ?? throw new ArgumentNullException(nameof(codeGenerationAssemblyProvider));
        }

        public IEnumerable<ITemplateDataProvider> DataProviders
        {
            get
            {
                var dataProviders = new List<ITemplateDataProvider>();
                foreach(var assembly in _codeGenerationAssemblyProvider.CandidateAssemblies)
                {
                    dataProviders.AddRange(assembly
                        .DefinedTypes
                        .Where(IsITemplateDataProvider)
                        .Select(typeInfo => CreateInstance(typeInfo)));
                }
                return dataProviders;
            }
        }

        private ITemplateDataProvider CreateInstance(TypeInfo typeInfo)
        {
            ITemplateDataProvider instance;
            try
            {
                instance = ActivatorUtilities.CreateInstance(_serviceProvider, typeInfo.AsType()) as ITemplateDataProvider;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(MessageStrings.DataProviderInstanceCreationError, typeInfo.FullName, ex.Message));
            }

            return instance;
        }

        public ITemplateDataProvider FindDataProvider(string dataContract)
        {
            if (dataContract == null)
            {
                throw new ArgumentNullException(nameof(dataContract));
            }

            var candidates = DataProviders
                .Where(provider => provider.DataContracts.Contains(dataContract, StringComparer.OrdinalIgnoreCase));

            var count = candidates.Count();

            if (count == 0)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture,
                        MessageStrings.DataProviderNotFound,
                        dataContract));
            }

            if (count > 1)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture,
                    MessageStrings.MultipleDataProvidersGenerators,
                    dataContract));
            }

            return candidates.First();
        }

        private bool IsITemplateDataProvider(TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }

            if (!typeInfo.IsClass ||
                typeInfo.IsAbstract ||
                typeInfo.ContainsGenericParameters)
            {
                return false;
            }

            return typeof(ITemplateDataProvider).GetTypeInfo().IsAssignableFrom(typeInfo);
        }
    }
}
