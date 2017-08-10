using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Templating.Razor.Compilation
{
    public class RoslynCompilationService : ICompilationService
    {
        public CompilationResult Compile(string content)
        {
            var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(content) };

            var references = MetadataReferenceProvider.Instance.MetadataReferences;

            var assemblyName = Path.GetRandomFileName();

            var compilation = CSharpCompilation.Create(assemblyName,
                        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                        syntaxTrees: syntaxTrees,
                        references: references);


            var result = GetAssemblyFromCompilation(compilation);
            if (result.Success)
            {
                var type = result.Assembly.GetExportedTypes()
                                   .First();

                return CompilationResult.Successful(string.Empty, type);
            }
            else
            {
                return CompilationResult.Failed(content, result.ErrorMessages);
            }
        }



        public static AssemblyGenerationResult GetAssemblyFromCompilation(
            CodeAnalysis.Compilation compilation)
        {
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms, pdbStream: null);

                if (!result.Success)
                {
                    var formatter = new DiagnosticFormatter();
                    var errorMessages = result.Diagnostics
                                            .Where(IsError)
                                            .Select(d => formatter.Format(d));

                    return AssemblyGenerationResult.FromErrorMessages(errorMessages);
                }

                ms.Seek(0, SeekOrigin.Begin);

                Assembly assembly;
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        ms.CopyTo(memoryStream);
                        assembly =  Assembly.Load(memoryStream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    var v = ex;
                    while (v.InnerException != null)
                    {
                        v = v.InnerException;
                    }
                    throw ex;
                }

                return AssemblyGenerationResult.FromAssembly(assembly);
            }
        }

        private static bool IsError(Diagnostic diagnostic)
        {
            return diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error;
        }
    }
}
