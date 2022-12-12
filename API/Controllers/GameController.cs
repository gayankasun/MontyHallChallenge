using API.Modal;
using Microsoft.AspNetCore.Mvc;
using MontyHallChallengeAPI.Modal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


namespace MontyHallChallenge.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class GameController : Controller
    {
        [HttpGet]
        [Route("new")]
        public Response RequestNewGame(int doorNumber, Guid? sessionID)
        {
            Guid sessionId = Guid.Empty;
            if (sessionID == Guid.Empty)
            {
                sessionId = GenerateSession();
            }
           
            return InitNewGame(doorNumber, sessionId);

        }


        [HttpPost]
        [Route("getResult")]
        public GameResult Simulator(GameRequest request)
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

            if (result.DN_with_Car == result.DN_after_shift_decision)
            {
                result.Result = Result.Won;
            }
            else
            {
                result.Result = Result.Lost;
            }

            //
            CalculateResult(result);

            return result;
        }

        private Response InitNewGame(int doorNumber, Guid sessionId)
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

            return new Response
            {
                RoundNumber = gameSummary.CurrentRound++,
                DN_with_Car = setCarIntoDoor,
                DN_host_going_to_open = setDoorHostOpen,
                SimulationType = SimulationType.single,
                SessionId = sessionId
            };
        }

        private Guid GenerateSession()
        {
            GameSummary gameSummary = new GameSummary()
            {
                SessionId = Guid.NewGuid(),
                CurrentRound = 1,
                WonCount = 0,
                LostCount = 0,
                WinningPercentage = 0
            };

            string json = JsonConvert.SerializeObject(gameSummary);
            string filePath = string.Format(@"C:\RnD\MontyHallChallenge\API\SessionProfiles\{0}.json", gameSummary.SessionId);
            System.IO.File.WriteAllText(filePath, json);

            return gameSummary.SessionId;
        }

        private void UpdateSession(GameSummary currentSummary)
        {

          GameSummary gameSummary =  GetCurrentGameSummary(currentSummary.SessionId);
            //GameSummary updatedResult = new GameSummary()
            //{
            //    CurrentRound = currentSummary.CurrentRound + 1,
            //    SessionId =currentSummary.SessionId,


            //}


            //decimal winningPercentage = 0;

            //winningPercentage = (currentSummary.Won / currentSummary.CurrentRound) * 100;
        }

        private void CalculateResult(GameResult currentGameResult)
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

            //currentGameSummary.WinningPercentage = 

            //UpdateSession(currentGameSummary);

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
