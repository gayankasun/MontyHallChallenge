using API.Modal;
using Microsoft.AspNetCore.Mvc;
using MontyHallChallengeAPI.Modal;
using Newtonsoft.Json;


namespace MontyHallChallenge.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class GameController : Controller
    {
        [HttpGet]
        [Route("new")]
        public Game RequestNewGame(int doorNumber, Guid CurrentSessionID)
        {

            Guid SessionId = Guid.Empty;
            if (CurrentSessionID == Guid.Empty)
            {
                SessionId = GenerateSession();
            }
            else
            {
                SessionId = CurrentSessionID;
            }

            return Game(doorNumber, SessionId, SimulationType.single);

        }

        [HttpPost]
        [Route("getResult")]
        public GameResult GetGameResult(GameRequest request)
        {
            return RunGame(request);
        }

        [HttpGet]
        [Route("autoPlay")]
        public GameSummary AutoPlayMode(int numOfRounds, bool isSwitch)
        {
            Guid sessionId = GenerateSession();

            for (int i = 1; i <= numOfRounds; i++)
            {
                int randomDoorPicked = Random.Shared.Next(1, 4);
                Game game = Game(randomDoorPicked, sessionId, SimulationType.Auto);

                GameRequest request = new GameRequest()
                {
                    RoundNumber = i,
                    ContestSelectedDoor = randomDoorPicked,
                    HostOpenedDoor = game.DN_host_going_to_open,
                    DoorWithCar = game.DN_with_Car,
                    SimulationType = (int)SimulationType.Auto,
                    IsSwitched = isSwitch,
                    SessionId = sessionId

                };
                RunGame(request);
            }

            return GetCurrentGameSummary(sessionId);
        }

        [HttpGet]
        [Route("customPlay")]
        public HttpResponseMessage CustomPlayMode(int numOfRounds, int numOfSets)
        {
            List<GamePercentageSummary> gameSummaryData = new List<GamePercentageSummary>();

            for (int i = 1; i <= numOfSets; i++)
            {
                Guid sessionId = GenerateSession();
                bool isSwitched = false;
                for (int k = 0; k < 2; k++)
                {
                    var x = k;

                    if (k == 1) { isSwitched = true; }

                    for (int j = 1; j <= numOfRounds; j++)
                    {
                        int randomDoorPicked = Random.Shared.Next(1, 4);
                        Game game = Game(randomDoorPicked, sessionId, SimulationType.Auto);

                        GameRequest request = new GameRequest()
                        {
                            RoundNumber = i,
                            ContestSelectedDoor = randomDoorPicked,
                            HostOpenedDoor = game.DN_host_going_to_open,
                            DoorWithCar = game.DN_with_Car,
                            SimulationType = (int)SimulationType.Auto,
                            IsSwitched = isSwitched,
                            SessionId = sessionId

                        };
                        RunGame(request);
                    }
                    GameSummary gameSummary = GetCurrentGameSummary(sessionId);
                    GamePercentageSummary meanSummary = new()
                    {
                        SessionId = gameSummary.SessionId,
                        WinPercentage = gameSummary.WinningPercentage,
                        LostPercentage = 100 - gameSummary.WinningPercentage,
                        IsSwitched = isSwitched
                    };

                    gameSummaryData.Add(meanSummary);
                }


            }

            decimal totalSwitchStrategyMean = 0;
            decimal totalKeepStrategyMean = 0;
            int totalRounds = numOfRounds * numOfSets;

            foreach (var game in gameSummaryData)
            {
                if (game.IsSwitched)
                {
                    totalSwitchStrategyMean = totalSwitchStrategyMean + game.WinPercentage;
                }
                else
                {
                    totalKeepStrategyMean = totalKeepStrategyMean + game.LostPercentage;
                }
            }
            GameMeanStrategy gameMeanStrategy = new()
            {
                TotalRounds = totalRounds,
                SwitchStrategyMean = Decimal.Round(((decimal)totalSwitchStrategyMean / totalRounds) * 100, 2),
                KeepStrategyMean = Decimal.Round(((decimal)totalKeepStrategyMean / totalRounds) * 100, 2),


            };

            //return new System.Net.HttpStatusCode)
            //{
            //    gameSummaryData= gameSummaryData,
            //    gameMeanStrategy = gameMeanStrategy
            //};
            return null;
        } 

        private Game Game(int doorNumber, Guid sessionId, SimulationType simulationType)
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

        private GameResult RunGame(GameRequest request)
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

        private Guid GenerateSession()
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
            string filePath = string.Format(@"C:\RnD\MontyHallChallenge\API\SessionProfiles\{0}.json", gameSummary.SessionId);
            System.IO.File.WriteAllText(filePath, json);

            return gameSummary.SessionId;
        }

        private GameSummary  UpdateSession(GameSummary currentSummary)
        {
           GameSummary gameSummary =  GetCurrentGameSummary(currentSummary.SessionId);
           var updatedGameSummary =     JsonConvert.SerializeObject(currentSummary);
           string filePath = string.Format(@"C:\RnD\MontyHallChallenge\API\SessionProfiles\{0}.json", gameSummary.SessionId);
           System.IO.File.WriteAllText(filePath, updatedGameSummary);
           
            return currentSummary;
        }

        private GameSummary CalculateResult(GameResult currentGameResult)
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
            currentGameSummary.WinningPercentage = Decimal.Round(((decimal)currentGameSummary.WonCount/currentGameSummary.Rounds) * 100, 2);

           return UpdateSession(currentGameSummary);

        }

        private GameSummary GetCurrentGameSummary(Guid sessionId)
        {
            string filePath = string.Format(@"C:\RnD\MontyHallChallenge\API\SessionProfiles\{0}.json", sessionId);
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
