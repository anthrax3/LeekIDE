using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using System.Windows;

namespace LeekIDE.Autocompletion.Errors
{
    class ErroredLine : DocumentColorizingTransformer
    {
        public ErroredLine(DocumentLine line)
        {
            CurrentLine = line;
        }
        protected DocumentLine CurrentLine { get; set; }
        protected override void ColorizeLine(DocumentLine line)
        {
            line = CurrentLine ?? line;
            var text = CurrentContext.Document.GetText(line);
            base.ChangeLinePart(line.Offset, line.EndOffset, visualLine =>
             {
                 var thing = new TextDecoration
                 {
                     Location = TextDecorationLocation.Underline,
                     Pen = new Pen(
                     new SolidColorBrush(
                         Color.FromRgb(
                             200,
                             20,
                             30)), 1)
                 };
                 visualLine.TextRunProperties.SetTextDecorations(new TextDecorationCollection(new List<TextDecoration> { thing }));
             }
             );
        }
    }
}
