namespace DogrudanTeminParadiseAPI.Parameters
{
    public class StatsQueryParameters
    {
        /// <summary>Period in days: e.g. 7, 30, 90, 365</summary>
        public int Days { get; set; }
        public int Top { get; set; } = 5;
    }
}
