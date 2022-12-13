namespace API.Modal
{
    public class GameSummary
    {
        public Guid SessionId { get; set; }
        public int Rounds { get; set; }
        public int WonCount { get; set; }
        public int LostCount { get; set; }
        public decimal WinningPercentage { get; set; }
    }


    public class GamePercentageSummary
    {
        public Guid SessionId { get; set; }
        public bool IsSwitched { get; set; }
        public decimal WinPercentage { get; set; }
        public decimal LostPercentage { get; set; }
    }

    public class GameMeanStrategy
    {
       public decimal SwitchStrategyMean { get; set; }
       public decimal KeepStrategyMean { get; set; }
       public int TotalRounds { get; set; }
    }


}
