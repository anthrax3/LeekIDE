using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace LeekIDE.Autocompletion.Data
{
    public class KeywordCompletion : ICompletionData
    {
        public KeywordCompletion(string text)
        {
            this.Text = text;
            try
            {
                CurrentBitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/keyword.png"));
                Image = CurrentBitmap;
            }
            catch (UriFormatException)
            {
                Debug.WriteLine("no image");
            }
        }
        private BitmapImage CurrentBitmap { get; set; }
        public System.Windows.Media.ImageSource Image { get; }
        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content => Text;

        public object Description => $"{Text} keyword";

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }

        public double Priority { get; } = 2;
    }
}