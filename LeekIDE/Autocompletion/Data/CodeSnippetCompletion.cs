using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace LeekIDE.Autocompletion.Data
{
    public class CodeSnippetCompletion : ICompletionData
    {
        public CodeSnippetCompletion(string shortened, string code)
        {
            this.Text = shortened;
            EntireCode = code;
            try
            {
                CurrentBitmap = new BitmapImage(new Uri("pack://application:,,,/LeekIDE;component/Resources/snippet.png"));
                Image = CurrentBitmap;
            }
            catch (UriFormatException)
            {
                Debug.WriteLine("no image");
            }
        }

        public CodeSnippetCompletion(CodeSnippet snippet)
        {
            Text = snippet.ShortenedCalling;
            try
            {
                CurrentBitmap = new BitmapImage(new Uri("pack://application:,,,/LeekIDE;component/Resources/snippet.png"));
                Image = CurrentBitmap;
            }
            catch (Exception)
            {
                Debug.WriteLine("no image");
            }
            EntireCode = snippet.Code;
        }
        private BitmapImage CurrentBitmap { get; set; }
        public System.Windows.Media.ImageSource Image { get; }
        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }
        public string EntireCode { get; set; }
        public object Description => $"A code snippet for {Text}";

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, EntireCode);
        }

        public double Priority { get; } = 1.5;
    }
}