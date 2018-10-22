using System.Collections.Generic;
using VenBowlScoring.Interface;
using System.Text;

namespace VenBowlScoring.Model
{
    public class Scorecard : IScorecard
    {
        public List<Player> JoinedPlayers;
        public Dictionary<Player, List<CommonFrame>> Sheet;

        public Scorecard(List<Player> players, int frameCount, int specialFrames)
        {
            Sheet = new Dictionary<Player, List<CommonFrame>>();
            List<CommonFrame> row = new List<CommonFrame>();


            //start of nFrame at specialFrames as we need a total of frameCount and yet specialFrames count needs to be a part of that.
            for (int nFrame = specialFrames; nFrame < frameCount; nFrame++)
            {
                row.Add(new CommonFrame());
            }
            //then add on the special frames at the end.
            for (int nFrame = 0; nFrame < specialFrames; nFrame++)
            {
                row.Add(new FinalFrame());
            }


            foreach (Player player in players)
            {
                Sheet.Add(player,row);
                throw new System.Exception("Line above the exception not implemented correctly. row needs to be copied not row...");

            }
        }

        public string Print(IPlayer player)
        {
            StringBuilder sbScores = new StringBuilder();
            List<CommonFrame> row = Sheet[(Player)player];
            for (int frameNumber = 1; frameNumber <= row.Count; frameNumber++) {
                sbScores.AppendFormat("Frame {0}: Ball 1: {1} Ball 1: {2} Score: {3}", frameNumber, row[0].FirstBallScore(), row[0].SecondBallScore(), row[0].FrameScore).ToString();
            }

            return sbScores.ToString();

        }

        public void CreateScorecard(List<IPlayer> Players, int frameCount, int specialFrameCount)
        {
            throw new System.NotImplementedException();
        }

        public void MarkScore(IPlayer player, int score)
        {
            throw new System.NotImplementedException();
        }
    }
}