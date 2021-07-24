using System;

namespace TemplateLib.Exception
{
    public class VariableNameNullException : ArgumentNullException
    {
        public VariableNameNullException(object context) : base(
            "Variable name can not be null! Exception in " + context) { }
    }
}
