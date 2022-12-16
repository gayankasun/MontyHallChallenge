using API.Modal;
using MontyHallChallengeAPI.Modal;
using Newtonsoft.Json;

namespace API.Core
{
    public class MontyHall : IMontyHall
    {
        private readonly IConfiguration _configuration;
        public MontyHall(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public Game InitGame(int doorNumber, Guid sessionId, SimulationType simulationType)
        {
            List<int> doorList = new List<int>() { 1, 2, 3 };
            int setCarIntoDoor = Random.Shared.Next(1, 4);
            doorList.RemoveAt(doorList.IndexOf(setCarIntoDoor));
            if (doorNumber != setCarIntoDoor)
            {
                doorList.RemoveAt(doorList.IndexOf(doorNumber));
            }


            int indexDoorHostOpen = Random.Shared.Next(doorList.Count);
            int setDoorHostOpen = doorList[indexDoorHostOpen];

            GameSummary gameSummary = GetCurrentGameSummary(sessionId);

            return new Game
            {
                RoundNumber = ++gameSummary.Rounds,
                DN_with_Car = setCarIntoDoor,
                DN_host_going_to_open = setDoorHostOpen,
                SimulationType = simulationType,
                SessionId = sessionId
            };
        }

        public GameResult RunGame(GameRequest request)
        {
            GameResult result = new GameResult();
            List<int> doorList = new List<int>() { 1, 2, 3 };

            if (request.IsSwitched)
            {
                doorList.RemoveAt(doorList.IndexOf(request.ContestSelectedDoor));
                doorList.RemoveAt(doorList.IndexOf(request.HostOpenedDoor));
                result.DN_after_shift_decision = doorList[0];
            }
            else
            {
                result.DN_after_shift_decision = request.ContestSelectedDoor;
            }

            result.DN_Contest_Choice = request.ContestSelectedDoor;
            result.DN_with_Car = request.DoorWithCar;
            result.IsSwitch = request.IsSwitched;
            result.SessionId = request.SessionId;
            result.RoundNumber = request.RoundNumber;

            if (result.DN_with_Car == result.DN_after_shift_decision)
            {
                result.Result = Result.Won;
            }
            else
            {
                result.Result = Result.Lost;
            }

            //
            result.GameSummary = CalculateResult(result);

            return result;
        }

        public Guid GenerateSession()
        {
            GameSummary gameSummary = new GameSummary()
            {
                SessionId = Guid.NewGuid(),
                Rounds = 0,
                WonCount = 0,
                LostCount = 0,
                WinningPercentage = 0
            };

            string json = JsonConvert.SerializeObject(gameSummary);
            var sessionProfilePath = _configuration.GetValue<string>("SessioProfile:filePath");
            string filePath = string.Format(@"{0}\{1}.json", sessionProfilePath, gameSummary.SessionId);
            System.IO.File.WriteAllText(filePath, json);

            return gameSummary.SessionId;
        }

        public GameSummary UpdateSession(GameSummary currentSummary)
        {
            GameSummary gameSummary = GetCurrentGameSummary(currentSummary.SessionId);
            var updatedGameSummary = JsonConvert.SerializeObject(currentSummary);
            var sessionProfilePath = _configuration.GetValue<string>("SessioProfile:filePath");
            string filePath = string.Format(@"{0}\{1}.json", sessionProfilePath, gameSummary.SessionId);
            System.IO.File.WriteAllText(filePath, updatedGameSummary);

            return currentSummary;
        }

        public GameSummary CalculateResult(GameResult currentGameResult)
        {
            GameSummary currentGameSummary = GetCurrentGameSummary(currentGameResult.SessionId);

            if (currentGameResult.Result == Result.Won)
            {
                currentGameSummary.WonCount++;
            }
            else
            {
                currentGameSummary.LostCount++;
            }
            currentGameSummary.Rounds++;
            currentGameSummary.WinningPercentage = Decimal.Round(((decimal)currentGameSummary.WonCount / currentGameSummary.Rounds) * 100, 2);

            return UpdateSession(currentGameSummary);

        }

        public GameSummary GetCurrentGameSummary(Guid sessionId)
        {
            var sessionProfilePath = _configuration.GetValue<string>("SessioProfile:filePath");
            string filePath = string.Format(@"{0}\{1}.json", sessionProfilePath, sessionId);
            GameSummary result = new GameSummary();
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                result = JsonConvert.DeserializeObject<GameSummary>(json);
            }

            return result;
        }
    }
}
