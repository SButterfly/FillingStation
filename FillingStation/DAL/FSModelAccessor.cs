using System;
using System.Windows.Forms;
using FillingStation.Core.Models;
using FillingStation.Localization;

namespace FillingStation.DAL
{
    public class FSModelAccessor
    {
        private readonly FSModelLoader _fsModelLoader;
        
        private const string _fileExt = ".fs";
        private const string _allFilesExt = ".*";

        private readonly string _dialogFilter;

        public FSModelAccessor()
        {
            _fsModelLoader = new FSModelLoader();

            _dialogFilter = CreateDialogFilter(new string[] { Strings.Filter_model, Strings.Filter_all},
                new string[] {_fileExt, _allFilesExt});
        }

        public static string DefaultFileName
        {
            get { return Strings.Filename; }
        }

        public bool Save(FileModel model)
        {
            if (String.IsNullOrEmpty(model.Path))
            {
                return SaveAs(model);
            }

            _fsModelLoader.Save(model.FSModel, model.Path);
            return true;
        }

        public bool SaveAs(FileModel model)
        {
            var dialog = new SaveFileDialog()
            {
                FileName = DefaultFileName,
                DefaultExt = _fileExt,
                Filter = _dialogFilter
            };

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string newPath = dialog.FileName;
                _fsModelLoader.Save(model.FSModel, newPath);

                model.Path = newPath;
                return true;
            }
            return false;
        }

        public FileModel Open()
        {
            var dialog = new OpenFileDialog()
            {
                FileName = DefaultFileName,
                DefaultExt = _fileExt,
                Filter = _dialogFilter
            };

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = dialog.FileName;
                var model = _fsModelLoader.Read(path);

                return new FileModel(model, path);
            }
            return null;
        }

        #region Helper methods

        private static string CreateDialogFilter(string[] extValues, string[] ext)
        {
            if (extValues.Length != ext.Length) throw new ArgumentException("Lengthes of arrays are different.");
            
            string result = "";
            for (int i = 0; i < ext.Length; i++)
            {
                result += string.Format("{0} (*{1})|*{1}", extValues[i], ext[i]);
                if (i < ext.Length - 1) result += '|';
            }
            return result;
        }

        #endregion
    }
}
