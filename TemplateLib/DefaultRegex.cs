using System;

namespace TemplateLib
{
    public static class DefaultRegex
    {
        public const string Regex = "%\\[([^%,\\s]+)\\]%";
        public const string DynamicVariableName = "DYN_VAR";
        public static readonly Func<string, string> SelectorFactory = name => $"%[{name}]%";
    }
}
