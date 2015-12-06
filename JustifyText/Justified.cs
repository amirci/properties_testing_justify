using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JustifyText
{
    public class Justified
    {
        public static string Justify(int width, string unjustified)
        {
            var words = unjustified.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            var justified = CreateLine(width, words, 0);

            return justified;
        }

        private static string CreateLine(int width, string[] words, int i)
        {
            if (i >= words.Length) return "";

            var line = new List<string>();
            var count = 0;
            Predicate<string> canFit = word => count + line.Count + word.Length <= width;
            Func<bool> isTheLastLine = () => i == words.Length;
            
            while (i < words.Length && canFit(words[i]) )
            {
                line.Add(words[i]);
                count += words[i].Length;
                i++;
            }

            var justified = isTheLastLine() ? string.Join(" ", line) : AdjustSpaces(line, width) + "\n";

            return justified + CreateLine(width, words, i);
        }

        private static string AdjustSpaces(List<string> words, int width)
        {
            var line = string.Join(" ", words);

            var spaceLeft = Math.Max(0, width - line.Length) + 1;

            return new Regex("\\s+").Replace(line, new string(' ', spaceLeft), 1);
        }
    }
}
