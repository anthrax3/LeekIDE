using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LeekIDE.Utilities
{
    public static class Extracter
    {
        public static IEnumerable<string> ExtractFromString(
    string text, string startString, string endString)
        {
            //int indexStart = 0, indexEnd = 0;
            //bool exit = false;
            //while (!exit)
            //{
            //    indexStart = text.IndexOf(startString);
            //    indexEnd = text.IndexOf(endString);
            //    if (indexStart != -1 && indexEnd != -1)
            //    {
            //        try
            //        {
            //            matched.Add(text.Substring(indexStart + startString.Length,
            //                                       indexEnd - indexStart - startString.Length));
            //            text = text.Substring(indexEnd + endString.Length);
            //        }
            //        catch
            //        {
            //            break;
            //        }
            //    }
            //    else
            //        exit = true;
            //}
            var regex = new Regex($"(?<={startString}).*?(?={endString})",RegexOptions.Singleline).Matches(text);
            foreach (Match match in regex)
            {
                yield return match.Value;
            }
        }
    }
}