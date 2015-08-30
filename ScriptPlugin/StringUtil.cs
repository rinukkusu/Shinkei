using System;
using System.Text.RegularExpressions;

namespace ScriptPlugin
{
    public static class StringUtil
    {
        public static string ReplaceLastOccurrence(this string source, string find, string replace)
        {
            int place = source.LastIndexOf(find, StringComparison.Ordinal);

            if (place == -1)
                return string.Empty;

            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

        public static string ReplaceFirstOccurrence(this string source, string find, string replace)
        {
            var regex = new Regex(Regex.Escape(find));
            return regex.Replace(source, replace, 1);
        } 
    }
}