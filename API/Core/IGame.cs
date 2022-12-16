using API.Modal;
using MontyHallChallengeAPI.Modal;

namespace API.Core
{
    public interface IMontyHall
    {
        Game InitGame(int doorNumber, Guid sessionId, SimulationType simulationType);
        GameResult RunGame(GameRequest request);
        Guid GenerateSession();
        GameSummary UpdateSession(GameSummary currentSummary);
        GameSummary CalculateResult(GameResult currentGameResult);
        GameSummary GetCurrentGameSummary(Guid sessionId);
    }
}
