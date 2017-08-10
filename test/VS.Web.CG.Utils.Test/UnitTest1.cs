using System;
using System.IO;
using Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.ProjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration.Core.Workspace;
using Newtonsoft.Json;
using Xunit;

namespace VS.Web.CG.Core.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

            var content = @"using System;
namespace Test.Dummy
{
  public class TestClass
  {
    
  }
}";
            var txt = File.ReadAllText(@"C:\Users\prbhosal\Desktop\projectcontext.json");

            var context = JsonConvert.DeserializeObject<CommonProjectContext>(txt);

            var manager = WorkspaceManager.Create(context);

            manager.AddDocumentToProject(
                context.ProjectFullPath,
                Path.Combine(Path.GetDirectoryName(context.ProjectFullPath), "Test.cs"),
                content);


            var compilation = manager.GetCompilationAsync().Result;

            if (compilation.Assembly != null)
            {

            }
        }
    }
}
