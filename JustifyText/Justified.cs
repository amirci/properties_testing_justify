using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JustifyText
{
    public class Justified
    {
        public static string Justify(int width, string unjustified)
        {
            var words = unjustified.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            return CreateLines(width, words);
        }

        private static Tuple<IReadOnlyCollection<string>, IReadOnlyCollection<string>> NextLine(IEnumerable<string> text, int width)
        {
            var line = new List<string>();

            var fitsAnotherWord = new Func<string, bool>(word =>
            {
                var wc = line.Count;

                line.Insert(0, word);

                return wc * 2 + word.Length <= width;
            });

            var rest = (IReadOnlyCollection<string>) text.SkipWhile(fitsAnotherWord).ToList();
            var roLine = (IReadOnlyCollection<string>) line;

            return Tuple.Create(roLine, rest);
        }

        private static string CreateLines(int width, IReadOnlyCollection<string> words)
        {
            if (words.Count == 0) return "";

            var nextLineInfo = NextLine(words, width);

            var line = nextLineInfo.Item1;
            var rest = nextLineInfo.Item2;

            var justified = rest.Count == 0 ? AddOneSpaceBetweenWords(line) : AdjustSpaces(line, width) + "\n";

            return justified + CreateLines(width, rest);
        }

        private static string AddOneSpaceBetweenWords(IEnumerable<string> words)
        {
            return string.Join(" ", words);
        }

        private static string AdjustSpaces(IEnumerable<string> words, int width)
        {
            var line = string.Join(" ", words);

            var spaceLeft = Math.Max(0, width - line.Length) + 1;

            return new Regex("\\s+").Replace(line, new string(' ', spaceLeft), 1);
        }
    }
}
