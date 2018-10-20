namespace VenBowlScoring.Interface
{
    interface IScorableFrame
    {
        void AddScore(int score, int scoreAttemptNum = 1);
        int CalculateScore();

    }
}
