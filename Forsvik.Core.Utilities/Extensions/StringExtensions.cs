using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forsvik.Core.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string text)
        {
            return !string.IsNullOrWhiteSpace(text);
        }

        public static string Truncate(this string text, int max)
        {
            if (text.Length > max)
            {
                text = text.Substring(0, max - 3) + "...";
            }
            return text;
        }
    }
}
