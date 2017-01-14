using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;

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
            startString = startString.Replace("(", "\\(").Replace(")", "\\)");
            endString = endString.Replace("(", "\\(").Replace(")", "\\)");
            var regex = new Regex($"(?<={startString}).*?(?={endString})", RegexOptions.Singleline).Matches(text);
            foreach (Match match in regex)
            {
                yield return match.Value;
            }
        }
        public static string GetPreviousWord(string what, int offset,int howMuchWords = 1)
        {
            var start = TextUtilities.GetNextCaretPosition(new TextEditor()
            {
                Text = what
            }.Document, offset,
              LogicalDirection.Backward, CaretPositioningMode.WordStart);
            var end = 0;
            var chars = new List<char>();
            if (start != -1)
            {
                end = offset;
                var current = start;
                while (current < end)
                {
                    if (current == -1)
                        break;
                    try
                    {
                        chars.Add(what[current]);
                    }
                    catch
                    {
                        break;
                    }
                    current++;
                }
            }
            var tempString = new string(chars.ToArray());
            if (howMuchWords > 1)
                tempString = GetPreviousWord(what.Remove(tempString.Length - 1,offset), chars.Count - 1, howMuchWords - 1);
            return tempString;
        }
        public static IEnumerable<(string str, int index)> ExtractFromStringTuple(
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
            startString = startString.Replace("(", "\\(").Replace(")", "\\)");
            endString = endString.Replace("(", "\\(").Replace(")", "\\)");
            var regex = new Regex($"(?<={startString}).*?(?={endString})", RegexOptions.Singleline).Matches(text);
            foreach (Match match in regex)
            {
                yield return (str: match.Value, index: match.Index);
            }
        }
        public static IEnumerable<string> ExtractFromString(
   string text, string[] startStrings, string[] endString)
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
            var group1 = startStrings.Aggregate((a, b) =>
            {
                return b.Replace("(", "\\(").Replace(")", "\\)") + ("|");
            });
            if ((group1.LastIndexOf("|") > 0))
                group1 = group1.Remove(group1.LastIndexOf("|"));
            var group2 = endString.Aggregate((a, b) =>
            {
                return b.Replace("(", "\\(").Replace(")", "\\)") + ("|");
            });
            if (group2.LastIndexOf("|") > 0)
                group2 = group2.Remove(group2.LastIndexOf("|"));
            var regex = new Regex($"(?<={group1}).*?(?={group2})", RegexOptions.Singleline).Matches(text);
            foreach (Match match in regex)
            {
                yield return match.Value;
            }
        }
        public static int GetBracketLevel(string text, int offset)
        {
            if (offset < 0 || offset >= text.Length)
                throw new ArgumentException(nameof(offset));
            int count = 0;
            for (var oldOffset = 0; oldOffset < offset; oldOffset++)
            {
                if (text[oldOffset] == '{')
                {
                    count++;
                }
                else if (text[oldOffset] == '}')
                {
                    count--;
                }
            }
            return count;
        }
    }
}