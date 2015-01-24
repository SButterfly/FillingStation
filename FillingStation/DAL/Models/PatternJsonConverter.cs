using System;
using FillingStation.Core.Patterns;
using FillingStation.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FillingStation.DAL.Models
{
    public class PatternJsonConverter : JsonCreationConverter<PatternModel>
    {
        private static readonly DoubleDictionary<Type, string> _dictionary = new DoubleDictionary<Type, string>{
            {typeof (CashBoxPattern), "cash"},
            {typeof (InfoTablePattern), "info"},
            {typeof (ColumnPattern), "column"},
            {typeof (TankPattern), "tank"},
            {typeof (RoadInPattern), "in"},
            {typeof (RoadOutPattern), "out"},
            {typeof (RoadTPattern), "roadt"},
            {typeof (RoadPattern), "road"},
            {typeof (RoadTurnPattern), "roadturn"}
        };

        protected override PatternModel Create(Type objectType, JObject jObject, JsonReader reader, JsonSerializer serializer)
        {
            JToken nameToken;
            JToken xToken;
            JToken yToken;
            jObject.TryGetValue("name", out nameToken);
            jObject.TryGetValue("x", out xToken);
            jObject.TryGetValue("y", out yToken);

            string name = nameToken.Value<string>();
            int x = xToken.Value<int>();
            int y = xToken.Value<int>();

            if (!_dictionary.ContainsValue(name))
            {
                Logger.WriteLine(this, "There is no name or this name doesn't supported");
                return null;
            }

            try
            {
                var pattern = (IPattern)Activator.CreateInstance(_dictionary.GetKey(name));

                //Create a new reader for this jObject, and set all properties to match the original reader.
                JsonReader jObjectReader = jObject.CreateReader();
                jObjectReader.Culture = reader.Culture;
                jObjectReader.DateParseHandling = reader.DateParseHandling;
                jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
                jObjectReader.FloatParseHandling = reader.FloatParseHandling;

                // Populate the object properties
                serializer.Populate(jObjectReader, pattern.Property);

                return new PatternModel(x, y, pattern);
            }
            catch (Exception e)
            {
                Logger.WriteLine(this, "An exeption was caught");
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var patternModel = value as PatternModel;
            if (patternModel != null)
            {
                var pattern = patternModel.Pattern;
                string name;
                if (!_dictionary.TryGetValue(pattern.GetType(), out name))
                {
                    Logger.WriteLine(this, "There is no name or this name doesn't supported: " + pattern.GetType());
                    return;
                }

                var jObject = JObject.FromObject(pattern.Property);
                jObject.Add("name", name);
                jObject.Add("x", patternModel.X);
                jObject.Add("y", patternModel.Y);
                jObject.WriteTo(writer);
            }
        }
    }
}