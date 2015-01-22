using System;
using System.Threading.Tasks;
using System.Windows;
using FillingStation.Localization;

namespace FillingStation.Helpers
{
    public abstract class MessageDialog : Window
    {
        private TaskCompletionSource<object> _taskCompletionSource;
        
        private object _result;
        private bool _isResultInitialized;
        protected virtual object Result
        {
            get { return _result; }
            set
            {
                _result = value;
                _isResultInitialized = true;
            }
        }

        public virtual Task<object> ShowAsync()
        {
            if (_taskCompletionSource == null)
                _taskCompletionSource = new TaskCompletionSource<object>();

            Closed += (sender, args) =>
            {
                if (_isResultInitialized || Result != null)
                {
                    _taskCompletionSource.SetResult(Result);
                }
                else
                {
                    _taskCompletionSource.SetCanceled();
                }
                _taskCompletionSource = null;
            };

            Show();

            return _taskCompletionSource.Task;
        }

        public void SetResultAndClose(object result)
        {
            Result = result;
            Close();
        }

        public static async Task<object> ShowAsync<T>() where T : MessageDialog, new()
        {
            return await new T().ShowAsync();
        }

        public static async Task<TR> ShowAsync<TR>(MessageDialog dialog)
        {
            return (TR)await dialog.ShowAsync();
        }

        public static async Task<TR> ShowAsync<TR, TS>() where TS : MessageDialog, new()
        {
            return (TR)await new TS().ShowAsync();
        }

        public static void ShowException(string text)
        {
            ShowException(text, Strings.ExceptionMessage_Title);
        }

        public static void ShowException(string text, string title)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowException(Exception e)
        {
            ShowException(e, Strings.ExceptionMessage_Title);
        }

        public static void ShowException(Exception e, string title)
        {
            ShowException(e.Message, title);
        }
    }
}