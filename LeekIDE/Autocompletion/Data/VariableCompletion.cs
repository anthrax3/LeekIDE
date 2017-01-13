using System;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace LeekIDE.Autocompletion.Data
{
    public class VariableCompletion : ICompletionData
    {
        public VariableCompletion(string text)
        {
            this.Text = text;
        }
        private BitmapImage CurrentBitmap { get; set; }
        public System.Windows.Media.ImageSource Image => new BitmapImage(new Uri("pack://application:,,,/LeekIDE;component/Resources/variable.png"));
        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }

        public object Description => $"{Text} variable";

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }

        public double Priority { get; } = 3;
    }
}