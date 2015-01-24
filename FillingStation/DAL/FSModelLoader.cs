using System;
using System.IO;
using System.Linq;
using FillingStation.Core.Models;
using FillingStation.DAL.Models;
using FillingStation.Localization;
using Newtonsoft.Json;

namespace FillingStation.DAL
{
    public class FSModelLoader
    {
        public void Save(FSModel model, string path)
        {
            try
            {
                var jsonModel = new FillingStationModel(model.Width, model.Height,
                model.Patterns.Select(pattern => new PatternModel(model.Get(pattern).X, model.Get(pattern).Y, pattern)));

                using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        var str = JsonConvert.SerializeObject(jsonModel, new PatternJsonConverter());
                        writer.Write(str);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(Strings.Exception_readFS + e.Message, e);
            }
        }


        public FSModel Read(string path)
        {
            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        var jsonModel = JsonConvert.DeserializeObject<FillingStationModel>(reader.ReadToEnd(), new PatternJsonConverter());
                        return jsonModel.CreateFSModel();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(Strings.Exception_readFS + e.Message, e);
            }
        }
    }
}
