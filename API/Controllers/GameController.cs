using API.Handlers;
using API.Modal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MontyHallChallengeAPI.Modal;
using Newtonsoft.Json;
using System.Net;

namespace MontyHallChallenge.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public GameController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        [HttpGet]
        [Route("new")]
        public APIJsonMessage RequestNewGame(int doorNumber, Guid CurrentSessionID)
        {
            APIJsonMessage rtnValue = new APIJsonMessage();
            try
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

                Game game = Game(doorNumber, SessionId, SimulationType.single);

                rtnValue.MessageBody.Content = new
                {
                    game
                };
            }
            catch (Exception ex)
            {
                rtnValue.StatusCode = HttpStatusCode.InternalServerError;

                rtnValue.MessageBody.IsSuccessful = false;
                rtnValue.MessageBody.Description = ex.Message;
            }

            return rtnValue;

         }

        [HttpPost]
        [Route("getResult")]
        public APIJsonMessage GetGameResult(GameRequest request)
        {
            APIJsonMessage rtnValue = new APIJsonMessage();
            try
            {
                GameResult gameResult = RunGame(request);

                rtnValue.MessageBody.Content = new
                {
                    gameResult
                };
            }
            catch (Exception ex)
            {
                rtnValue.StatusCode = HttpStatusCode.InternalServerError;

                rtnValue.MessageBody.IsSuccessful = false;
                rtnValue.MessageBody.Description = ex.Message;
            }

            return rtnValue;
        }

        [HttpGet]
        [Route("autoPlay")]
        public APIJsonMessage AutoPlayMode(int numOfRounds, bool isSwitch)
        {
            APIJsonMessage rtnValue = new APIJsonMessage();

            try
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

                GameSummary gameSummary = GetCurrentGameSummary(sessionId);

                rtnValue.MessageBody.Content = new
                {
                    gameSummary
                };
            }
            catch (Exception ex)
            {
                rtnValue.StatusCode = HttpStatusCode.InternalServerError;

                rtnValue.MessageBody.IsSuccessful = false;
                rtnValue.MessageBody.Description = ex.Message;
            }

            return rtnValue;
        }

        [HttpGet]
        [Route("customPlay")]
        public APIJsonMessage CustomPlayMode(int numOfRounds, int numOfSets)
        {
            APIJsonMessage rtnValue = new APIJsonMessage();

            try
            {
                List<GamePercentageSummary> gameSummaryData = new List<GamePercentageSummary>();

                for (int i = 1; i <= numOfSets; i++)
                {
                    Guid sessionId = GenerateSession();
                    bool isSwitched = false;
                    for (int k = 0; k < 2; k++)
                    {
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

                decimal totalWinSwitchStrategyMean = 0;
                decimal totalWinKeepStrategyMean = 0;
                int totalRounds = numOfRounds * numOfSets;

                List<decimal> listWinPercentageIfSwitch = new List<decimal>();
                List<decimal> listWinPercentageIfKeep = new List<decimal>();
                List<int> listNumOfSets = new List<int>();
                
                foreach (var game in gameSummaryData)
                {
                    if (game.IsSwitched)
                    {
                        listWinPercentageIfSwitch.Add(game.WinPercentage);
                       // totalWinSwitchStrategyMean = totalWinSwitchStrategyMean + game.WinPercentage;
                    }
                    else
                    {
                        listWinPercentageIfKeep.Add(game.WinPercentage);
                        //totalWinKeepStrategyMean = totalWinKeepStrategyMean + game.WinPercentage;
                    }

                }

                for (int i = 1; i <= numOfSets; i++)
                {
                    listNumOfSets.Add(i);
                }

                var a = totalWinSwitchStrategyMean;
                var b = totalWinKeepStrategyMean;

                var c = Decimal.Round(((decimal)totalWinSwitchStrategyMean / numOfRounds), 2);
                var d = Decimal.Round(((decimal)totalWinKeepStrategyMean / numOfRounds), 2);

                GameMeanStrategy gameMeanStrategy = new()
                {
                    TotalRounds = totalRounds,
                    SwitchStrategyMean = Decimal.Round(((decimal)totalWinSwitchStrategyMean / totalRounds) * 100, 2),
                    KeepStrategyMean = Decimal.Round(((decimal)totalWinKeepStrategyMean / totalRounds) * 100, 2),

                };




                rtnValue.MessageBody.Content = new
                {
                    //gameSummaryData = gameSummaryData,
                    //gameMeanStrategy = gameMeanStrategy
                    listWinPercentageIfSwitch = listWinPercentageIfSwitch,
                    listWinPercentageIfKeep = listWinPercentageIfKeep,
                    numOfRoundsList= listNumOfSets,
                    noOfSetsCount = numOfSets

                };
            }
            catch (Exception ex)
            {
                rtnValue.StatusCode = HttpStatusCode.InternalServerError;

                rtnValue.MessageBody.IsSuccessful = false;
                rtnValue.MessageBody.Description = ex.Message;
            }

            return rtnValue;
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
            var sessionProfilePath = _configuration.GetValue<string>("SessioProfile:filePath");
            string filePath = string.Format(@"{0}\{1}.json", sessionProfilePath , gameSummary.SessionId);
            System.IO.File.WriteAllText(filePath, json);

            return gameSummary.SessionId;
        }

        private GameSummary  UpdateSession(GameSummary currentSummary)
        {
           GameSummary gameSummary =  GetCurrentGameSummary(currentSummary.SessionId);
           var updatedGameSummary =     JsonConvert.SerializeObject(currentSummary);
           var sessionProfilePath = _configuration.GetValue<string>("SessioProfile:filePath");
           string filePath = string.Format(@"{0}\{1}.json", sessionProfilePath, gameSummary.SessionId);
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
            var sessionProfilePath = _configuration.GetValue<string>("SessioProfile:filePath");
            string filePath = string.Format(@"{0}\{1}.json",sessionProfilePath, sessionId);
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
