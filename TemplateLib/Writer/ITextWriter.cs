using System.Collections.Generic;

namespace TemplateLib.Writer
{
    public interface ITextWriter
    {
        string Template { get; set; }
        ITextWriter Copy();
        string ToWriting(Dictionary<string, string> variables, string defaultValue);
    }
}