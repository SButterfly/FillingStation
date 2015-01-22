using System;
using FillingStation.Localization;

namespace FillingStation.Core.Generators
{
    public class GeneratorDetermin : IGenerator
    {
        private readonly double _dt;

        public GeneratorDetermin(double dt)
        {
            if (dt <= 0)
                throw new ArgumentOutOfRangeException("dt", "dt " + Strings.Exception_positive_int);

            _dt = dt;
        }

        public double Next()
        {
            return _dt;
        }
    }
}
