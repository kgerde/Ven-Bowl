using System.Collections.Generic;
using VenBowlScoring.Model;

namespace VenBowlScoring.Interface
{
    interface IGame
    {
        void StartGame();

        void AddPlayers(IEnumerable<Player> players);

        void RemovePlayers(IEnumerable<Player> players);

        void Play(IBowlAttempt nextAttempt);

        void FinishGame();

    }
}
