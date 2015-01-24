using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FillingStation.Core.Models;
using Newtonsoft.Json;

namespace FillingStation.DAL.Models
{
    public class FillingStationModel
    {
        public FillingStationModel()
        {
            
        }

        public FillingStationModel(int width, int height, IEnumerable<PatternModel> patterns)
        {
            Width = width;
            Height = height;
            Patterns = patterns.ToList();
        }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("patterns")]
        public List<PatternModel> Patterns { get; set; } 

        public FSModel CreateFSModel()
        {
            var model = new FSModel(Width, Height);
            foreach (var pattern in Patterns)
            {
               model.Add(pattern.Pattern, new Point(pattern.X, pattern.Y));
            }
            return model;
        }
    }
}