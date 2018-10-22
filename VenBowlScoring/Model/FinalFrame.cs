using VenBowlScoring.Constants;
using VenBowlScoring.Exceptions;

namespace VenBowlScoring.Model
{
    /// <summary>
    /// Allows for Special frames to be added at the end of the game. The default purpose of this class is to support the final frame. of a standard game of bowling.
    /// </summary>
    public class FinalFrame : CommonFrame
    {
        #region properties

        /// <summary>
        /// How many balls per frame. by default this is 3.
        /// </summary>
        public new static int BallCount { get; internal set; } = Constant.DEFAULT_SPECIAL_FRAMES_BALLS;

        /// <summary>
        /// Strikes are calculated differently in the final frame. When set to true this property will change the frame logic to that of a final frame.
        /// <TODO>
        /// Not implemented.
        /// </TODO>
        /// </summary>
        public new static bool StrikeContinueLogic = true;

        /// <summary>
        /// Strikes are calculated differently in the final frame. When set to true this property will change the frame logic to that of a final frame.
        /// <TODO>
        /// Not implemented.
        /// </TODO>
        /// </summary>
        public new static bool SpareContinueLogic = true;

        #endregion

        #region Methods

        /// <summary>
        /// Confirm the next two balls have been bowled. Used by the strike scoring calculations.
        /// </summary>
        /// <returns>True if the next two balls have been bowled, otherwise false.</returns>
        protected new bool NextTwoBallsBowled()
        {
            //if there are any more frames
            if (NextTwoFrames.Count > 1)
            {
                //Then the next frame may have some of the needed balls. Otherwise the balls will be in this frame and will be handled by the Frame Status Handler code.
                if (null != NextTwoFrames[0].BallScores[0] && NextTwoFrames[0].BallScores.Count == 2 && null != NextTwoFrames[0].BallScores[1])
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// The state engine controller. this method sets all of the boolean values to allow for description of the frame's game state.
        /// </summary>
        public new void HandleFrameStatus()
        {
            if (!IsScoringComplete)
            {
                if (null != PreviousFrame && !PreviousFrame.IsScoringComplete)
                {
                    //Update the previous frame
                    PreviousFrame.HandleFrameStatus();
                }
                else
                {
                    IsPreviousFrameScored = true;
                }

                if (BallScores.Count == 3)
                {
                    IsClosed = IsSpare(BallScores) || BallScores[0].IsStrike();
                    IsReadyForNextFrame = true;
                    IsReadyToScore = NextTwoFrames.Count == 0 || (NextTwoFrames.Count > 0 && IsClosed && NextTwoBallsBowled());
                }
                //handle frame status on second ball
                if (BallScores.Count == 2)
                {
                    IsClosed = false;
                    IsReadyForNextFrame = !BallScores[0].IsStrike() && !IsSpare(BallScores);
                    IsReadyToScore = (IsPreviousFrameScored && IsReadyForNextFrame);
                }
                //handle frame status on first ball
                else if (BallScores.Count == 1)
                {
                    bool isStrikeOnFault = (BallScores[0].IsFault() && LostPinsByFault == Constant.NUMBER_OF_PINS);
                    IsClosed = false;
                    IsReadyForNextFrame = (isStrikeOnFault);
                    IsReadyToScore = (IsPreviousFrameScored && IsReadyForNextFrame);
                }

                if (IsReadyToScore)
                {
                    CalculateScore();
                    IsScoringComplete = true;
                }
            }
            else
            {
                IsPreviousFrameScored = true;
            }
        }

        /// <summary>
        /// The method to calculate the score of a frame.
        /// </summary>
        /// <returns>integer Score of the frame</returns>
        /// <exception cref="FrameNotReadyToScoreException">FrameNotReadyToScoreException</exception>
        public new int CalculateScore()
        {
            int score = null != PreviousFrame ? PreviousFrame.FrameScore : 0;
            if (IsReadyToScore)
            {
                for (int ball = 0; ball < FinalFrame.BallCount; ball++)
                {
                    score += BallScores[ball].Value;
                    //If the ball made a spare then the next ball also needs to be added to the ball.
                    if (IsSpare(BallScores))
                    {
                        //find the next ball
                        BowlingNumber nextBall = (null != BallScores[ball + 1]) ? BallScores[ball + 1] : (
                            (null != NextTwoFrames && null != NextTwoFrames[0].BallScores) ? NextTwoFrames[0].BallScores[0] : null);
                        if (null != nextBall)
                        {
                            score += nextBall.Value;
                        }

                    }
                    //else if the ball is a strick calculate it two balls forward.
                    else if (BallScores[ball].IsStrike())
                    {
                        //find the next ball and add the scores.
                        BowlingNumber nextBall = (BallScores.Count > ball + 1) ? BallScores[ball + 1] :
                            ((NextTwoFrames.Count > 0 && NextTwoFrames[0].BallScores.Count > 0) ? NextTwoFrames[0].BallScores[0] : null);

                        score += null != nextBall ? nextBall.Value : 0;

                        //find the next next ball and add the scores.
                        BowlingNumber nextNextBall = new BowlingNumber();
                        if (BallScores.Count > (ball + 2))
                        {
                            nextNextBall = BallScores[ball + 2];
                        }
                        else if ((NextTwoFrames.Count > 0 && (NextTwoFrames[0].BallScores.Count > 0)))
                        {
                            nextNextBall = (NextTwoFrames[0].BallScores.Count > 1) ? NextTwoFrames[0].BallScores[1] : new BowlingNumber();
                        }

                        if (nextNextBall.Value > 0)
                        {
                            score += nextNextBall.Value;
                        }
                    }
                }
                FrameScore = score;
                return FrameScore;
            }
            else
            {
                throw new FrameNotReadyToScoreException("Frame is not ready to be scored. Please continue to bowl.");
            }
        }

        /// <summary>
        /// Pretty print the third ball and handle the spare test case as it is a frame level concept.
        /// </summary>
        /// <returns>string bowling text version of scores.</returns>
        public string ThirdBallScore()
        {
            if (null != BallScores[2])
            {
                if (IsSpare(BallScores))
                {
                    return "/";
                }
                else
                {
                    return BallScores[2].Text;
                }
            }
            else
            {
                return "";
            }
        }

        #endregion

    }
}
