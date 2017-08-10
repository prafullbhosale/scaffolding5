﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Templating.Razor
{
    public abstract class RazorTemplateBase
    {
        private TextWriter Output { get; set; }

        public dynamic Model { get; set; }

        public abstract Task ExecuteAsync();

        public async Task<string> ExecuteTemplate()
        {
            StringBuilder output = new StringBuilder();
            using (var writer = new StringWriter(output))
            {
                Output = writer;
                await ExecuteAsync();
            }
            return output.ToString();
        }

        public void WriteLiteral(object value)
        {
            WriteLiteralTo(Output, value);
        }

        public virtual void WriteLiteralTo(TextWriter writer, object text)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (text != null)
            {
                writer.Write(text.ToString());
            }
        }

        public virtual void Write(object value)
        {
            WriteTo(Output, value);
        }

        public virtual void WriteTo(TextWriter writer, object content)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (content != null)
            {
                writer.Write(content.ToString());
            }
        }

        public void BeginContext(int position, int length, bool x)
        {
            // Do Nothing.
        }

        public void EndContext()
        {
            // Do Nothing.
        }
    }
}
