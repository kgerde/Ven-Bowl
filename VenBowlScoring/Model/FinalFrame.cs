using VenBowlScoring.Constants;
using VenBowlScoring.Exceptions;

namespace VenBowlScoring.Model
{
    public class FinalFrame : CommonFrame
    {
        public new static int BallCount { get; internal set; } = Constant.DEFAULT_SPECIAL_FRAMES_BALLS;

        public new static bool StrikeContinueLogic = true;
        public new static bool SpareContinueLogic = true;

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
                        BowlingNumber nextBall = (BallScores.Count >ball + 1) ? BallScores[ball + 1] :
                            ((NextTwoFrames.Count > 0 && NextTwoFrames[0].BallScores.Count > 0) ? NextTwoFrames[0].BallScores[0] : null);

                        score += null != nextBall ? nextBall.Value : 0;

                        //find the next next ball and add the scores.
                        BowlingNumber nextNextBall = new BowlingNumber();
                        if (BallScores.Count > (ball+2))
                        {
                            nextNextBall = BallScores[ball + 2];
                        }
                        else if ((NextTwoFrames.Count >0 && (NextTwoFrames[0].BallScores.Count > 0)))
                        {
                            nextNextBall = (NextTwoFrames[0].BallScores.Count > 1) ?  NextTwoFrames[0].BallScores[1] : new BowlingNumber();
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

        //private bool NextTwoBallsBowled()
        //{
        //    //This could be the last frame
        //    if(this.NextTwoFrames.Conut == 0)
        //    {
        //        this.BallScores
        //    }
        //    //if there is only one more frame
        //    if (this.NextTwoFrames.Count == 1)
        //    {
        //        //Then the frame needs to have two balls bowled.
        //        if (null != this.NextTwoFrames[0].BallScores[0] && this.NextTwoFrames[0].BallScores.Count == 2 && null != this.NextTwoFrames[0].BallScores[1])
        //        {
        //            return true;
        //        }
        //    }
        //    //otherwise there should be two frames or more left.
        //    else if (this.NextTwoFrames.Count == 2)
        //    {
        //        //In thiscase the balls could be in the next frame or in the next two frames.
        //        if (null != this.NextTwoFrames[0].BallScores[0] && this.NextTwoFrames[0].BallScores.Count == 2 && null != this.NextTwoFrames[0].BallScores[1])
        //        {
        //            return true;
        //        }
        //        if (null != this.NextTwoFrames[0].BallScores[0] && this.NextTwoFrames[0].BallScores.Count == 1 && null != this.NextTwoFrames[1].BallScores[0])
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}


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
    }
}
