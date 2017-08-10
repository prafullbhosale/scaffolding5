using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyModel;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Templating.Razor
{
    internal class MetadataReferenceProvider
    {

        public static MetadataReferenceProvider Instance = new MetadataReferenceProvider();

        private MetadataReferenceProvider()
        {
            MetadataReferences = GetApplicationReferences();
        }

        public IEnumerable<MetadataReference> MetadataReferences { get; }

        private static IEnumerable<MetadataReference> GetApplicationReferences()
        {
            var references = new List<MetadataReference>();
            var depsFiles = AppContext.GetData("APP_CONTEXT_DEPS_FILE") as string;
            var files = depsFiles?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            var application = files != null && files.Length > 0 ? files[0] : null;

            using (var stream = File.OpenRead(application))
            {
                var dependencyContext = new DependencyContextJsonReader().Read(stream);
                var libraries = dependencyContext
                    .GetRuntimeAssemblyNames(dependencyContext.Target.Runtime)
                    .Select(x => Assembly.Load(x.FullName).Location);

                foreach (var library in libraries)
                {
                    try
                    {
                        var metadata = AssemblyMetadata.CreateFromFile(library);

                        references.Add(metadata.GetReference());
                    }
                    catch (Exception ex)
                        when (ex is NotSupportedException
                        || ex is ArgumentException
                        || ex is BadImageFormatException
                        || ex is IOException)
                    {
                        continue;
                    }
                }

            }

            return references;
        }
    }
}
