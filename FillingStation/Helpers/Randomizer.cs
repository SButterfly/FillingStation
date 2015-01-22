using System;

namespace FillingStation.Helpers
{
    public sealed class Randomizer
    {
        private static Randomizer _randomizer;

        private Randomizer()
        {
            Random = new Random();
        }

        public Random Random { get; private set; }

        static readonly object _sync = new object();
        public static Randomizer GetInstance()
        {
            if (_randomizer == null)
            {
                lock (_sync)
                {
                    if (_randomizer == null)
                    {
                        _randomizer = new Randomizer();
                    }
                }
            }
            return _randomizer;
        }
    }
}