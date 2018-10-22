using System.Collections.Generic;
using System.Text;
using VenBowlScoring.Interface;

namespace VenBowlScoring.Model
{
    public class Scorecard : IScorecard
    {
        public List<Player> JoinedPlayers;
        public Dictionary<Player, List<CommonFrame>> Sheet;
        public Dictionary<Player, CommonFrame> CurrentFrames;

        public Scorecard(List<Player> players, int frameCount, int specialFrames)
        {
            Sheet = new Dictionary<Player, List<CommonFrame>>();
            List<CommonFrame> row = new List<CommonFrame>();
            CurrentFrames = new Dictionary<Player, CommonFrame>();


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

            //Add each player with their own version of the row.
            foreach (Player player in players)
            {
                List<CommonFrame> originalRowPerPlayer = new List<CommonFrame>();
                foreach (CommonFrame frame in row)
                {
                    originalRowPerPlayer.Add((CommonFrame)frame.Clone());

                }
                Sheet.Add(player, originalRowPerPlayer);

                //update the CurrentFrames object so that a player can play through the frames.
                CurrentFrames.Add(player, Sheet[players[0]][0]);

                //Update all next two frames and previous frames.
                for (int bindCount = 0; bindCount < frameCount; bindCount++)
                {
                    int nextIndex = bindCount + 1;
                    int nextNextIndex = bindCount + 2;
                    if (nextIndex < frameCount) {
                        originalRowPerPlayer[bindCount].NextTwoFrames.Add(originalRowPerPlayer[nextIndex]);
                    }
                    if (nextNextIndex < frameCount)
                    {
                        originalRowPerPlayer[bindCount].NextTwoFrames.Add(originalRowPerPlayer[nextNextIndex]);
                    }
                    int previousIndex = bindCount - 1;
                    if (0 <= previousIndex) {
                        originalRowPerPlayer[bindCount].PreviousFrame = originalRowPerPlayer[previousIndex];
                    }
                }


            }
            //            CurrentFrame = Sheet.GetValueOrDefault(Sheet.Keys[0])[0];
        }

        public string Print(IPlayer player)
        {
            StringBuilder sbScores = new StringBuilder();
            List<CommonFrame> row = Sheet[(Player)player];
            for (int frameNumber = 1; frameNumber <= row.Count; frameNumber++)
            {
                sbScores.AppendFormat("Frame {0}: Ball 1: {1} Ball 2: {2} Score: {3}", frameNumber, row[0].FirstBallScore(), row[0].SecondBallScore(), row[0].FrameScore).ToString();
            }

            return sbScores.ToString();

        }

        public void MarkScore(Player player, int score)
        {
            if(CurrentFrames[player].IsReadyForNextFrame)
            {
                CurrentFrames[player] = CurrentFrames[player].NextTwoFrames[0];
            }
            Sheet[player][Sheet[player].IndexOf(CurrentFrames[player])].MarkScore(score);
        }
    }
}