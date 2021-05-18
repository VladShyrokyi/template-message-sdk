using System;
using TemplateLib.Objects;

namespace TemplateConsoleApp
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var template = new Template("%[VAR]%", "%\\[([^%,\\s]+)\\]%", a => a);
            Console.Out.WriteLine(template);
            while (true)
            {

            }
        }
    }
}
