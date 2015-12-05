using System;
using System.Collections.Generic;

namespace JustifyText
{
    public class Justified
    {
        public static string Justify(int width, string unjustified)
        {
            var words = unjustified.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            var justified = CreateLine(width, words, 0);

            var splitted = justified.Split('\n');

            return justified;
        }

        private static string CreateLine(int width, string[] words, int i)
        {
            if (i >= words.Length) return "";

            var line = new List<string>();
            var count = 0;

            while (i < words.Length && count + words[i].Length <= width)
            {
                line.Add(words[i]);
                count += words[i].Length;
                i++;
            }

            var endLine = i < words.Length ? "\n" : "";

            return string.Join(" ", line) + endLine + CreateLine(width, words, i);
        }
    }
}
