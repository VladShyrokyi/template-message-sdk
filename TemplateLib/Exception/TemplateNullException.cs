using System;

namespace TemplateLib.Exception
{
    public class TemplateNullException : ArgumentNullException
    {
        public TemplateNullException(object context) : base("Template can not be null! Exception in " + context) { }
    }
}
