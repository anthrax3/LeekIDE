using System;
using ICSharpCode.AvalonEdit;

namespace LeekIDE.Autocompletion.Syntaxic
{
    public static class Symbols
    {
        public static char? AutoCompleteBrackets(TextEditor edit, char e)
        {
            switch (e)
            {
                case '{':
                    edit.TextArea.PerformTextInput("\n\t\n}"); // escaping ;o
                    edit.CaretOffset -= 2;
                    return '}';
                case '(':
                    edit.TextArea.PerformTextInput(")");
                    edit.CaretOffset--;
                    return ')';
                case '"':                    
                    edit.TextArea.PerformTextInput("\"");
                    edit.CaretOffset--;
                    return '"';
                default:
                    return null;
            }
        }
    }
}