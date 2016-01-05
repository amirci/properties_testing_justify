using System;
using System.Collections.Generic;
using System.Linq;

namespace JustifyText
{
    public class Justified
    {
        public static string Justify(int width, string unjustified)
        {
            var words = unjustified.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            return CreateLines(width, words);
        }

        private static Tuple<IReadOnlyCollection<string>, IReadOnlyCollection<string>, int> TakeLine(IReadOnlyCollection<string> text, int width)
        {
            var line = new List<string>();
            var charCount = 0;

            var fitsAnotherWord = new Func<string, bool>(word =>
            {
                var spaces = line.Count;

                var fitsAnother = charCount + word.Length + spaces <= width;

                if (fitsAnother)
                {
                    line.Add(word);
                    charCount += word.Length;
                }

                return fitsAnother;
            });

            var rest = text.SkipWhile(fitsAnotherWord).ToList();

            return Tuple.Create((IReadOnlyCollection<string>) line,
                (IReadOnlyCollection<string>) rest,
                charCount);
        }

        private static string CreateLines(int width, IReadOnlyCollection<string> words)
        {
            if (words.Count == 0) return "";

            var info = TakeLine(words, width);

            var line = info.Item1;
            var rest = info.Item2;
            var charCount = info.Item3;

            var justified = rest.IsEmpty() 
                ? OneSpaceBetween(line) 
                : DistributeSpaces(line, width, charCount);

            return justified + CreateLines(width, rest);
        }

        private static string OneSpaceBetween(IEnumerable<string> words)
        {
            return string.Join(" ", words);
        }

        private static string DistributeSpaces(IReadOnlyCollection<string> words, int width, int charCount)
        {
            if (words.Count == 1) return words.First();

            int reminder;
            var totalSpaces = width - charCount;
            var div = Math.DivRem(totalSpaces, words.Count - 1, out reminder) ;
            var spaces = new string(' ', div);
            Func<string> extra = () => reminder-- > 0 ? " " : string.Empty;

            return words.Aggregate((line, word) => line + spaces + extra() + word) + "\n";
        }
    }

    public static class EnumerableExtensions
    {
        public static bool IsEmpty<T>(this IReadOnlyCollection<T> collection)
        {
            return collection.Count == 0;
        }

        public static int SumBy<T>(this IEnumerable<T> collection, Func<T, int> projection)
        {
            return collection.Aggregate(0, (acc, e) => acc + projection(e));
        }
    }
}
