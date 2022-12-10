using API.Modal;
using Microsoft.AspNetCore.Mvc;
using MontyHallChallengeAPI.Modal;
using System.Reflection.Metadata.Ecma335;

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
            return new Response
            {
                RoundNumber = 1,
                DN_with_Car = Random.Shared.Next(1,4),
                SimulationType = SimulationType.single
            };
        }

        [HttpPost]
        [Route("Request")]
        public Game Simulator(SimulateRequest request)
        {
            Game result = new Game();

            result.DN_of_Player_Choice = request.SelectedDoor;
            result.DN_with_Car = Random.Shared.Next(3);


            if (request.SimulationType == (int)SimulationType.single)
            {


                //result = new Game
                //{
                //    RoundNumber = 1,
                //    DN_with_Car = Random.Shared.Next(1, 4),
                //    DN_of_Choice = Random.Shared.Next(1, 4),
                //    IsSwitch = true,
                //    Result = Result.Lost
                //};
            }

            return result;
        }


        private void Run(int playerPickedDoor)
        {
            bool IsPlayerWin = false;
            int probDoorForGoatX;
            int probDoorForGoatY;


            switch (playerPickedDoor)
            {
                case 1: probDoorForGoatX = 2; probDoorForGoatY = 3; break;
                case 2: probDoorForGoatX = 1; probDoorForGoatY = 3; break;
                case 3: probDoorForGoatX = 1; probDoorForGoatY = 2; break;
            }



       
        }
    }
}
