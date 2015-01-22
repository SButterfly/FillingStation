using System;
using FillingStation.Helpers;
using FillingStation.Localization;

namespace FillingStation.Core.Generators
{
    public class GeneratorNormal : IGenerator
    {
        private readonly double _mx;
        private readonly double _dx;
        private readonly Random _random;

        public GeneratorNormal(double mx, double dx)
        {
            if (mx <= 0)
                throw new ArgumentOutOfRangeException("mx", "mx " + Strings.Exception_positive_int);
            if (dx <= 0)
                throw new ArgumentOutOfRangeException("dx", "dx " + Strings.Exception_positive_int);
            if (mx - Math.Sqrt(dx) <= 0)
                throw new ArgumentException(Strings.Exeption_Normal);

            _mx = mx;
            _dx = dx;
            _random = Randomizer.GetInstance().Random;
        }

        public double Next()
        {
            double sigma = Math.Sqrt(_dx);
            double dsumm = 0d;

            for (int i = 0; i <= 12; i++)
            {
                dsumm += _random.NextDouble();
            }
            return Math.Round((_mx + sigma*(dsumm - 6)), 3);
        }
    }
}
