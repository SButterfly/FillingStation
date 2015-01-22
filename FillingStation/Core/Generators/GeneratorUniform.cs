using System;
using FillingStation.Helpers;
using FillingStation.Localization;

namespace FillingStation.Core.Generators
{
    internal class GeneratorUniform : IGenerator
    {
        private readonly double _a;
        private readonly double _b;
        private readonly Random _random;

        public GeneratorUniform(double a, double b)
        {
            if (a <= 0)
                throw new ArgumentOutOfRangeException("a", "a " + Strings.Exception_positive_int);
            if (b <= 0)
                throw new ArgumentOutOfRangeException("a", "b " + Strings.Exception_positive_int);
            if (b <= a)
                throw new ArgumentOutOfRangeException("a must be smaller than b", null as Exception);

            _a = a;
            _b = b;
            _random = Randomizer.GetInstance().Random;
        }

        public double Next()
        {
            return _a + (_b - _a)*_random.NextDouble();
        }
    }
}
