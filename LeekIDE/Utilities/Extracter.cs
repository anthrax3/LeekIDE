using System.Collections.Generic;

namespace LeekIDE.Utilities
{
    public static class Extracter
    {
        public static List<string> ExtractFromString(
    string text, string startString, string endString)
        {
            List<string> matched = new List<string>();
            int indexStart = 0, indexEnd = 0;
            bool exit = false;
            while (!exit)
            {
                indexStart = text.IndexOf(startString);
                indexEnd = text.IndexOf(endString);
                if (indexStart != -1 && indexEnd != -1)
                {
                    try
                    {
                        matched.Add(text.Substring(indexStart + startString.Length,
                                                   indexEnd - indexStart - startString.Length));
                        text = text.Substring(indexEnd + endString.Length);
                    }
                    catch
                    {
                        break;
                    }
                }
                else
                    exit = true;
            }
            return matched;
        }
    }
}