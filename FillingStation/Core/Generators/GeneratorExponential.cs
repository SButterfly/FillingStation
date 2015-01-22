using System;
using FillingStation.Helpers;
using FillingStation.Localization;

namespace FillingStation.Core.Generators
{
    public class GeneratorExponential : IGenerator
    {
        private readonly double _y;
        private readonly Random _random;

        public GeneratorExponential(double y)
        {
            if (y <= 0)
                throw new ArgumentOutOfRangeException("y", "y " + Strings.Exception_positive_int);

            _y = y;
            _random = Randomizer.GetInstance().Random;
        }

        public double Next()
        {
            double mx = 1d/_y;
            double sigma = 1d/_y;
            double dsumm = 0;

            for (int i = 0; i <= 12; i++)
            {
                dsumm += _random.NextDouble();
            }
            return Math.Round((mx + sigma*(dsumm - 6)), 3);
        }
    }
}
