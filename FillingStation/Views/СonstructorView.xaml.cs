using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FillingStation.Annotations;
using FillingStation.Core.Models;
using FillingStation.DAL;
using FillingStation.Helpers;
using FillingStation.Localization;

namespace FillingStation.Views
{
    public partial class СonstructorView : Window, INotifyPropertyChanged
    {
        public СonstructorView()
        {
            //TODO remove
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en");

            InitializeComponent();
            DataContext = this;

            Closing += (sender, args) =>
            {
                var result = SaveProjectIfNeeded();
                if (result == MessageBoxResult.Cancel)
                    args.Cancel = true;
            };
        }

        #region Properties

        public FSModel FSModel
        {
            get { return FSFileModel == null ? null : FSFileModel.FSModel; }
        }

        private FileModel _fsFileModel;
        public FileModel FSFileModel
        {
            get { return _fsFileModel; }
            set
            {
                if (Equals(value, _fsFileModel)) return;

                _fsFileModel = value;

                if (FSModel != null)
                {
                    FSModel.Changed += (sender, args) => { IsProjectSaved = false; };
                }

                if (_fsFileModel == null)
                {
                    IsProjectSaved = true;
                }

                constructionControl.FSModel = FSModel;

                OnPropertyChanged();
                OnPropertyChanged("FSModel");
                OnPropertyChanged("IsProjectOpened");
                OnPropertyChanged("PageTitle");
            }
        }
        public bool IsProjectOpened
        {
            get { return FSFileModel != null && FSFileModel.FSModel != null; }
        }

        private bool _isProjectSaved = true;
        public bool IsProjectSaved
        {
            get { return _isProjectSaved; }
            set
            {
                if (value.Equals(_isProjectSaved)) return;

                _isProjectSaved = value;

                OnPropertyChanged();
                OnPropertyChanged("PageTitle");
            }
        }

        public string PageTitle
        {
            get
            {
                if (!IsProjectOpened)
                {
                    return Strings.ProgramName;
                }

                return "[" + (FSFileModel.Path ?? Strings.Filename_notSaved) + (!IsProjectSaved ? "*" : "") + "]";
            }
        }

        #endregion

        #region Methods

        private void OpenSimulationView(object sender, EventArgs args)
        {
            try
            {
                if (FSModel.IsCorrect())
                {
                    var stream = constructionControl.GetImageStream();
                    var view = new SimulationView(FSModel, stream);
                    OpenView(view);
                }
            }
            catch (Exception e)
            {
                MessageDialog.ShowException(e);
            }
        }

        private void OpenCarsDBView(object sender, EventArgs args)
        {
            try
            {
                var view = new CarsDBView();
                OpenView(view);
            }
            catch (Exception e)
            {
                MessageDialog.ShowException(e);
            }
        }

        private void OpenFuelDBView(object sender, EventArgs args)
        {
            try
            {
                var view = new FuelDBView();
                OpenView(view);
            }
            catch (Exception e)
            {
                MessageDialog.ShowException(e);
            }
        }

        private void OpenHelpView(object sender, EventArgs args)
        {
            try
            {
                var view = new HtmlView(Strings.Help, "Assets\\help.html");
                OpenView(view);
            }
            catch (Exception e)
            {
                MessageDialog.ShowException(e);
            }
        }

        private void OpenCreditsView(object sender, EventArgs args)
        {
            try
            {
                var view = new HtmlView(Strings.Help, "Assets\\credits.html");
                OpenView(view);
            }
            catch (Exception e)
            {
                MessageDialog.ShowException(e);
            }
        }

        private void OpenView(Window view)
        {
            try
            {
                IsEnabled = false;
                view.Closed += (s, a) => IsEnabled = true;
                view.Show();
            }
            catch
            {
                IsEnabled = true;
                throw;
            }
        }

        private async void CreateProject(object sender, EventArgs args)
        {
            var result = SaveProjectIfNeeded();
            if (result == MessageBoxResult.Cancel)
                return;

            try
            {
                IsEnabled = false;
                var model = await MessageDialog.ShowAsync<FSModelSettings>() as FSModel;
                FSFileModel = new FileModel(model);
                IsEnabled = true;
            }
            catch (TaskCanceledException e)
            {
                IsEnabled = true;
            }
            catch (Exception e)
            {
                IsEnabled = true;
                MessageDialog.ShowException(e);
            }
        }

        private void SaveProject(object sender, EventArgs args)
        {
            try
            {
                var loader = new FSModelAccessor();

                var result = loader.Save(FSFileModel);
                IsProjectSaved = result;
            }
            catch (Exception e)
            {
                MessageDialog.ShowException(e);
            }
        }

        private void SaveAsProject(object sender, EventArgs args)
        {
            try
            {
                var loader = new FSModelAccessor();

                var result = loader.SaveAs(FSFileModel);
                IsProjectSaved = result;
            }
            catch (Exception e)
            {
                MessageDialog.ShowException(e);
            }
        }

        private void LoadProject(object sender, EventArgs args)
        {
            var result = SaveProjectIfNeeded();
            if (result == MessageBoxResult.Cancel)
                return;

            try
            {
                var loader = new FSModelAccessor();

                var resultFileModel = loader.Open();
                if (resultFileModel != null)
                {
                    FSFileModel = resultFileModel;
                }
            }
            catch (Exception e)
            {
                MessageDialog.ShowException(e);
            }
        }

        private void CloseProject(object sender, EventArgs args)
        {
            var result = SaveProjectIfNeeded();
            if (result == MessageBoxResult.Cancel)
                return;

            FSFileModel = null;
        }

        private MessageBoxResult SaveProjectIfNeeded()
        {
            if (!IsProjectSaved)
            {
                var result = MessageBox.Show(this, Strings.SaveProject, Strings.Attention, MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        SaveProject(this, null);
                    }
                    catch (Exception e)
                    {
                        return MessageBoxResult.Cancel;
                    }
                }
                return result;
            }
            return MessageBoxResult.Yes;
        }

        private void СonstructorView_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                SaveProject(sender, e);
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
            {
                LoadProject(sender, e);
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.N)
            {
                CreateProject(sender, e);
            }
            if (e.Key == Key.F5)
            {
                OpenSimulationView(sender, e);
            }
        }

        #endregion

        #region Binding

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
