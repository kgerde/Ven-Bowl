using VenBowlScoring.Model;

namespace VenBowlScoring.Interface
{
    public interface IScorecard
    {
        string Print(IPlayer player);
        void MarkScore(Player player, int score);

    }
}
