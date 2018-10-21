using System.Collections.Generic;
using VenBowlScoring.Model;

namespace VenBowlScoring.Interface
{
    interface IPlayer
    {
        List<string> FindGames(string gameFilter = null);
        void JoinGame();
        void TakeTurn();
        void ReviewHistory();
    }
}
