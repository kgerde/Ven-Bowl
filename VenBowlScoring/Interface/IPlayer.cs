using System.Collections.Generic;
using VenBowlScoring.Model;

namespace VenBowlScoring.Interface
{
    public interface IPlayer
    {
        List<string> FindGames(string gameFilter = null);
        void HostGame(string gameName);
        void JoinGame(Game game);
        string ReviewHistory();
        int TakeTurn(int score = 10);

    }
}
