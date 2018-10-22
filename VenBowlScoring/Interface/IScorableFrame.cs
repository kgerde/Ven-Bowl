using System;

namespace VenBowlScoring.Interface
{
    public interface IScorableFrame : ICloneable
    {
        void MarkScore(int score);
        int CalculateScore();

    }
}
