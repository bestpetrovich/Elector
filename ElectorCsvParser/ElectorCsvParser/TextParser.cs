using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ElectorCsvParser
{
    internal static class TextParser
    {
        internal static string GetNextWord(string str, int markerIndex, string marker)
        {
            var text = str.Substring(markerIndex+marker.Length);

            var words = GetWords(text);

            return words[0];
        }

        static string[] GetWords(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"\b[\w/\w//]*\b");

            var words = from m in matches.Cast<Match>()
                        where !string.IsNullOrEmpty(m.Value)
                        select TrimSuffix(m.Value);

            return words.ToArray();
        }

        static string TrimSuffix(string word)
        {
            int apostropheLocation = word.IndexOf('\'');
            if (apostropheLocation != -1)
            {
                word = word.Substring(0, apostropheLocation);
            }

            return word;
        }
    }
}