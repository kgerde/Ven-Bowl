using System;
using System.Collections.Generic;
using System.Text;
using VenBowlScoring.Model;

namespace VenBowlScoring.Interface
{
    public interface IScorecard
    {
        string Print(IPlayer player);
        void MarkScore(Player player, int score);

    }
}
