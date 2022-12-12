namespace MontyHallChallengeAPI.Modal
{
    public class GameRequest
    {
        public int RoundNumber { get; set; }
        public int ContestSelectedDoor { get; set; }
        public int HostOpenedDoor { get; set; }
        public int DoorWithCar { get; set; }
        public int SimulationType { get; set; }
        public bool IsSwitched { get; set; }
        public Guid SessionId { get; set; }


    }

}
