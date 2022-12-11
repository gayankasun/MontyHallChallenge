namespace MontyHallChallengeAPI.Modal
{
    public class GameResult
    {
        public int RoundNumber { get; set; }
        public int DN_with_Car { get; set; }
        public int DN_Contest_Choice { get; set; }
        public int DN_after_shift_decision { get; set; }
        public bool IsSwitch { get; set; }
        public Result Result { get; set; }

    }

    public enum Result
    {
        Lost = 0,
        Won = 1
    }

    public enum SimulationType
    {
       single= 1,
       Auto = 2,
       Custom = 3
    }
}
