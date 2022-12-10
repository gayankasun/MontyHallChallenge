namespace MontyHallChallengeAPI.Modal
{
    public class Game
    {
        public int RoundNumber { get; set; }
        public int DN_with_Car { get; set; }
        public int DN_of_Player_Choice { get; set; }
        public int DN_of_Host_shows { get; set; }
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
