using System;
using System.Collections.Generic;
using System.Text;

namespace VenBowlScoring.Interface
{
    public interface IScorecard
    {
        string Print(IPlayer player);
        void CreateScorecard(List<IPlayer> Players, int frameCount, int specialFrameCount);
        void MarkScore(IPlayer player, int score);

    }
}
