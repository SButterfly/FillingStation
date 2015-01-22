using System;

namespace FillingStation.Helpers
{
    public interface IAverage<TNum, TDen, out TRes>
    {
        TNum LastNumerator { get; }
        TDen LastDenominator { get; }

        TNum SumNumerator { get; }
        TDen SumDenominator { get; }
        TRes AVG { get; }

        void Add(TNum numerator, TDen denominator);
    }

    public class AverageTimeSpan : IAverage<TimeSpan, int, TimeSpan>
    {
        public TimeSpan LastNumerator { get; private set; }
        public int LastDenominator { get; private set; }

        public TimeSpan SumNumerator { get; private set; }
        public int SumDenominator { get; private set; }
        public TimeSpan AVG { get { return new TimeSpan(SumDenominator == 0 ? 0 : SumNumerator.Ticks / SumDenominator); } }

        public void Add(TimeSpan numerator, int denominator)
        {
            SumNumerator += numerator;
            SumDenominator += denominator;
            LastNumerator = numerator;
            LastDenominator = denominator;
        }
    }

    public class AverageDouble : IAverage<double, double, double>
    {
        public double LastNumerator { get; private set; }
        public double LastDenominator { get; private set; }

        public double SumNumerator { get; private set; }
        public double SumDenominator { get; private set; }
        public double AVG { get { return SumDenominator == 0 ? 0 : SumNumerator / SumDenominator; } }

        public void Add(double numerator, double denominator)
        {
            SumNumerator += numerator;
            SumDenominator += denominator;
            LastNumerator = numerator;
            LastDenominator = denominator;
        }
    }
}