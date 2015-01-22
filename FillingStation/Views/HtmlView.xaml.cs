using System;
using System.IO;
using System.Windows;
using FillingStation.Localization;

namespace FillingStation.Views
{
    public partial class HtmlView : Window
    {
        public HtmlView(string title, string path)
        {
            InitializeComponent();
            Title = title;

            string exePath = Directory.GetCurrentDirectory();
            string htmlPath = Path.Combine(exePath, path);

            if (!File.Exists(htmlPath))
            {
                throw new Exception(string.Format(Strings.Exception_HtmlNotFoundFormat, htmlPath));
            }
            browser.Navigate(new Uri(htmlPath, UriKind.Absolute));
        }
    }
}
