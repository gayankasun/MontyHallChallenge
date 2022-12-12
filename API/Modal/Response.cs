using MontyHallChallengeAPI.Modal;

namespace API.Modal
{
    public class Response
    {
        public int RoundNumber { get; set; }
        public SimulationType SimulationType { get; set; }
        public int DN_with_Car { get; set; }
        public int DN_host_going_to_open { get; set; }
        public Guid SessionId { get; set; }

    }
}
