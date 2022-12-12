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
}
