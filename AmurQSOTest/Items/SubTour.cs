namespace AmurQSOTest.Items
{
    /// <summary>
    /// подтур
    /// </summary>
    public class SubTour
    {
        /// <summary>
        /// номер подтура
        /// </summary>
        public int Number;
        /// <summary>
        /// длительность подтура
        /// </summary>
        public Period Period;
        /// <summary>
        /// тур заканчивающий период. для решения проблем с концом типа 2000 которые по сути другой тур
        /// </summary>
        public bool End;

        public SubTour()
        {
            Period = new Period();
        }

        public SubTour(int number, Period period)
        {
            Number = number;
            Period = period;
        }

        public SubTour(int number, Period period, bool end) : this(number, period)
        {
            End = end;
        }
    }
}