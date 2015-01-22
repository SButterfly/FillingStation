using FillingStation.Core.Properties;

namespace FillingStation.Core.Patterns
{
    public interface IPattern
    {
        int Height { get; }
        int Width { get; }

        string ImagePath { get; }

        IProperty Property { get; }
    }
}