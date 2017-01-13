using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace LeekIDE.Autocompletion.Data
{
    public class FunctionCompletion : ICompletionData
    {
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text + "()");
            textArea.Caret.Offset--;
        }

        public FunctionCompletion(string func)
        {
            Text = func;
        }
        public ImageSource Image => new BitmapImage(new Uri("pack://application:,,,/LeekIDE;component/Resources/function.png"));
        public string Text { get; }
        public object Content => Text;
        public object Description => $"{Text} function";
        public double Priority => 3;
    }
} 