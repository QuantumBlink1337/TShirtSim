using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TShirtSim
{
    internal class Utility
    {
        private static readonly Regex whitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(string input, string replacement)
        {
            return whitespace.Replace(input, replacement);
        }
    }
}
