using System.Collections.Generic;
using System.Linq;

namespace TemplateTest.Utils
{
    public static class TextHelper
    {
        public static string CreateTextWithVariables(Dictionary<string, string> variables,
                                                     string textBeginning = "Text:")
        {
            return variables.Aggregate(textBeginning,
                (container, pair) => container + $"\n - variable {pair.Key} = {pair.Value}"
            );
        }
    }
}
