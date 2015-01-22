namespace FillingStation.Helpers
{
    public struct Pair<TFirst, TSecond>
    {
        public Pair(TFirst first, TSecond second)
            : this()
        {
            First = first;
            Second = second;
        } 

        public TFirst First { get; set; }
        public TSecond Second { get; set; }
    }
}