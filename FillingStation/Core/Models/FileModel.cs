using System;

namespace FillingStation.Core.Models
{
    public class FileModel
    {
        public FileModel(FSModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            FSModel = model;
        }

        public FileModel(FSModel model, string path)
            : this(model)
        {
            Path = path;
        }

        public string Path { get; set; }
        public FSModel FSModel { get; set; }
    }
}