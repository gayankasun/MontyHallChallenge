using API.Core;
using API.Handlers;
using API.Modal;
using Microsoft.AspNetCore.Mvc;
using MontyHallChallengeAPI.Modal;
using System.Net;

namespace MontyHallChallenge.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IMontyHall montyHall ;
        public GameController(IConfiguration _configuration, IMontyHall _montyHall)
        {
            this.configuration = _configuration;
            this.montyHall = _montyHall;
        }

        /// <summary>
        /// This endpoint intiate new Game and response with Session Data
        /// </summary>
        /// <param name="doorNumber"></param>
        /// <param name="CurrentSessionID"></param>
        /// <returns>Single Game instance</returns>
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
                    SessionId = this.montyHall.GenerateSession();
                }
                else
                {
                    SessionId = CurrentSessionID;
                }

                Game game = this.montyHall.InitGame(doorNumber, SessionId, SimulationType.single);

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

        /// <summary>
        /// This endpoint return individual game results
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Game Results</returns>
        [HttpPost]
        [Route("getResult")]
        public APIJsonMessage GetGameResult(GameRequest request)
        {
            APIJsonMessage rtnValue = new APIJsonMessage();
            try
            {
                GameResult gameResult = this.montyHall.RunGame(request);

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

        /// <summary>
        /// This Endpoint can simulate given number of Rounds for single session
        /// </summary>
        /// <param name="numOfRounds"></param>
        /// <param name="isSwitch"></param>
        /// <returns>Game Summary</returns>

        [HttpGet]
        [Route("autoPlay")]
        public APIJsonMessage AutoPlayMode(int numOfRounds, bool isSwitch)
        {
            APIJsonMessage rtnValue = new APIJsonMessage();

            try
            {
                Guid sessionId = this.montyHall.GenerateSession();

                for (int i = 1; i <= numOfRounds; i++)
                {
                    int randomDoorPicked = Random.Shared.Next(1, 4);
                    Game game = this.montyHall.InitGame(randomDoorPicked, sessionId, SimulationType.Auto);

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
                    this.montyHall.RunGame(request);
                }

                GameSummary gameSummary = this.montyHall.GetCurrentGameSummary(sessionId);

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

        /// <summary>
        /// This Endpoint can simulate given number of sets with 3 Door 
        /// </summary>
        /// <param name="numOfRounds"></param>
        /// <param name="numOfSets"></param>
        /// <returns></returns>
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
                    Guid sessionId = this.montyHall.GenerateSession();
                    bool isSwitched = false;
                    for (int k = 0; k < 2; k++)
                    {
                        if (k == 1) { isSwitched = true; }

                        for (int j = 1; j <= numOfRounds; j++)
                        {
                            int randomDoorPicked = Random.Shared.Next(1, 4);
                            Game game = this.montyHall.InitGame(randomDoorPicked, sessionId, SimulationType.Auto);

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
                            this.montyHall.RunGame(request);
                        }
                        GameSummary gameSummary = this.montyHall.GetCurrentGameSummary(sessionId);
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

                int totalRounds = numOfRounds * numOfSets;

                List<decimal> listWinPercentageIfSwitch = new List<decimal>();
                List<decimal> listWinPercentageIfKeep = new List<decimal>();
                List<int> listNumOfSets = new List<int>();
                
                foreach (var game in gameSummaryData)
                {
                    if (game.IsSwitched)
                    {
                        listWinPercentageIfSwitch.Add(game.WinPercentage);
                    }
                    else
                    {
                        listWinPercentageIfKeep.Add(game.WinPercentage);
                    }

                }

                for (int i = 1; i <= numOfSets; i++)
                {
                    listNumOfSets.Add(i);
                }

                rtnValue.MessageBody.Content = new
                {
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

    }
}
