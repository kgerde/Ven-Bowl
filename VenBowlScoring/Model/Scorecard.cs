using System.Collections.Generic;
using VenBowlScoring.Interface;

namespace VenBowlScoring.Model
{
    public class Scorecard : IScorecard
    {
        public List<Player> JoinedPlayers;
        private int FramesPerGame = 10;

        public Scorecard()
        {

        }



    }
}