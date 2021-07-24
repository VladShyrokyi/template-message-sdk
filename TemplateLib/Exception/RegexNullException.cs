using System;

namespace TemplateLib.Exception
{
    public class RegexNullException : ArgumentNullException
    {
        public RegexNullException(object context) : base("Regex can not be null! Exception in " + context) { }
    }
}
