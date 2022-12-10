using API.Modal;
using Microsoft.AspNetCore.Mvc;
using MontyHallChallengeAPI.Modal;
using System.Diagnostics;


namespace MontyHallChallenge.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class GameController : Controller
    {
        [HttpGet]
        [Route("new")]
        public Response RequestNewGame()
        {
            List<int> doorList = new List<int>() { 1, 2, 3 };
            int setCarIntoDoor = Random.Shared.Next(1, 4);
            doorList.RemoveAt(doorList.IndexOf(setCarIntoDoor));

            int indexDoorHostOpen = Random.Shared.Next(doorList.Count);
            int setDoorHostOpen = doorList[indexDoorHostOpen];


            return new Response
            {
                RoundNumber = 1,
                DN_with_Car = setCarIntoDoor,
                DN_host_will_open = setDoorHostOpen,
                SimulationType = SimulationType.single
            };
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
                result.DN_Result_shows = doorList[0];
            }
            else
            {
                result.DN_Result_shows = request.ContestSelectedDoor;
            }

            result.DN_Contest_Choice = request.ContestSelectedDoor;
            result.DN_with_Car = request.DoorWithCar;
            result.IsSwitch = request.IsSwitched;

      

            if (result.DN_Result_shows == request.ContestSelectedDoor )
            {
                result.Result = Result.Won;
            }
            else
            {
                result.Result = Result.Lost;
            }

            return result;
        }

    }
}
