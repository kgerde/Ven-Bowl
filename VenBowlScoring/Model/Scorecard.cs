using System.Collections.Generic;
using System.Text;
using VenBowlScoring.Interface;

namespace VenBowlScoring.Model
{
    public class Scorecard : IScorecard
    {
        #region properties

        /// <summary>
        /// List of all players
        /// </summary>
        public List<Player> JoinedPlayers;

        /// <summary>
        /// Sheet of frames per player.
        /// </summary>
        public Dictionary<Player, List<CommonFrame>> Sheet;

        /// <summary>
        /// Player keyed current frames
        /// <TODO>
        /// This is not the best pattern to handle this the cursors for what Frame each player is currently on it should be replaced.
        /// </TODO>
        /// </summary>
        public Dictionary<Player, CommonFrame> CurrentFrames;

        #endregion

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="players">List of all Players on the score card.</param>
        /// <param name="frameCount">number of frames to include on the score card.</param>
        /// <param name="specialFrames">number of final frames on the score card.</param>
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
                    if (nextIndex < frameCount)
                    {
                        originalRowPerPlayer[bindCount].NextTwoFrames.Add(originalRowPerPlayer[nextIndex]);
                    }
                    if (nextNextIndex < frameCount)
                    {
                        originalRowPerPlayer[bindCount].NextTwoFrames.Add(originalRowPerPlayer[nextNextIndex]);
                    }
                    int previousIndex = bindCount - 1;
                    if (0 <= previousIndex)
                    {
                        originalRowPerPlayer[bindCount].PreviousFrame = originalRowPerPlayer[previousIndex];
                    }
                }


            }
        }

        /// <summary>
        /// Prints the scores for a player.
        /// </summary>
        /// <param name="player">IPlayer used to filter the data by one player per print.</param>
        /// <returns>string of scores in a Frame based string format.</returns>
        public string Print(IPlayer player)
        {
            StringBuilder sbScores = new StringBuilder();
            List<CommonFrame> row = Sheet[(Player)player];
            sbScores.AppendFormat("{0}", ((Player)player).Name);
            sbScores.AppendLine();

            for (int frameNumber = 1; frameNumber <= row.Count; frameNumber++)
            {
                sbScores.AppendFormat("Frame {0}: ", frameNumber);
                sbScores.AppendLine();
                for (int ballCounter = 1; ballCounter <= CommonFrame.BallCount; ballCounter++)
                {
                    string ballScore = "";

                    if (ballCounter == 1)
                    {
                        ballScore = row[frameNumber - 1].FirstBallScore();
                    }
                    else if (ballCounter == 2)
                    {
                        ballScore = row[frameNumber - 1].SecondBallScore();
                    }
                    else if (ballCounter == 3)
                    {
                        ballScore = ((FinalFrame)row[frameNumber - 1]).ThirdBallScore();
                    }

                    sbScores.AppendFormat("Ball {0}: {1} ", ballCounter, ballScore);

                }
                sbScores.AppendLine();
                sbScores.AppendFormat(" Score: {0}", row[frameNumber - 1].FrameScore.ToString());
            }

            return sbScores.ToString();

        }

        /// <summary>
        /// record a ball score for a player in the current frame
        /// </summary>
        /// <param name="player">Player player reporting the score</param>
        /// <param name="score">integer ball score between -X and X where X is the number of pins in the game. negative numbers allow for faults to be reported.</param>
        public void MarkScore(Player player, int score)
        {
            //Is the current frame supposed to be updated or does it need to be moved to the next frame?
            if (CurrentFrames[player].IsReadyForNextFrame)
            {
                if (CurrentFrames[player].NextTwoFrames.Count > 0 && null != CurrentFrames[player].NextTwoFrames[0])
                {
                    CurrentFrames[player] = CurrentFrames[player].NextTwoFrames[0];
                }
            }

            //update the score on the current frame.
            Sheet[player][Sheet[player].IndexOf(CurrentFrames[player])].MarkScore(score);
        }
        #endregion

    }
}