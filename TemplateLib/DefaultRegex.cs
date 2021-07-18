namespace TemplateLib
{
    public static class DefaultRegex
    {
        public const string Regex = "%\\[([^%,\\s]+)\\]%";
        public const string DynamicVariableName = "DYN_VAR";

        public static string CreateSelector(string name)
        {
            return $"%[{name}]%";
        }
    }
}
