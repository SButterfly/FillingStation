using System.Windows;
using FillingStation.Core.Models;
using FillingStation.Helpers;
using FillingStation.Localization;

namespace FillingStation.Views
{
    public partial class FSModelSettings : MessageDialog
    {
        private const int _minHeight = 10;
        private const int _maxHeight = 20;

        private const int _minWidth = 20;
        private const int _maxWidth = 30;

        private const int _defaultHeight = 10;
        private const int _defaultWidth = 20;

        public FSModelSettings()
        {
            InitializeComponent();

            widthTextBox.Text = _defaultWidth.ToString();
            heightTextBox.Text = _defaultHeight.ToString();

            okButton.Focus();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            int width, height;

            if (!int.TryParse(widthTextBox.Text, out width))
            {
                ShowPropertyError(Strings.Width);
                return;
            }
            if (!int.TryParse(heightTextBox.Text, out height))
            {
                ShowPropertyError(Strings.Height);
                return;
            }

            if (_minHeight <= height && height <= _maxHeight && _minWidth <= width && width <= _maxWidth)
            {
                SetResultAndClose(new FSModel(width, height));
            }
            else
            {
                ShowException(string.Format(Strings.Exception_FSModelRangeFormat,
                                        _minWidth, _maxWidth,
                                        _minHeight, _maxHeight));
            }
        }

        private void ShowPropertyError(string property)
        {
            ShowException(string.Format(Strings.Exception_properyFormat, property));
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
