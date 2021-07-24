using System;

namespace TemplateLib.Exception
{
    public class VariableNullException : ArgumentNullException
    {
        public VariableNullException(object context) :
            base("Variable can not be null! Exception in " + context) { }
    }
}
