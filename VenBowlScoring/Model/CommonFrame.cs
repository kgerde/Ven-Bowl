using System;
using System.Collections.Generic;
using VenBowlScoring.Constants;
using VenBowlScoring.Exceptions;
using VenBowlScoring.Interface;

namespace VenBowlScoring.Model
{
    public class CommonFrame : ICommonFrame
    {
        public static int BallCount { get; internal set; } = Constant.DEFAULT_FRAMES_BALLS;

        public static bool StrikeContinueLogic = false;
        public static bool SpareContinueLogic = false;

        public List<CommonFrame> NextTwoFrames = new List<CommonFrame>();
        public CommonFrame PreviousFrame;


        public List<BowlingNumber> BallScores { get; protected set; } = new List<BowlingNumber>();
        public int FrameScore { get; protected set; }
        public bool IsClosed { get; protected set; } = false;
        public bool IsReadyForNextFrame { get; protected set; } = false;
        public bool IsReadyToScore { get; protected set; } = false;
        public bool IsPreviousFrameScored { get; protected set; } = false;

        public int LostPinsByFault { get; protected set; }
        public bool IsScoringComplete { get; protected set; }

        public CommonFrame()
        {
        }

        public CommonFrame(CommonFrame previousFrame)
        {
            PreviousFrame = previousFrame;
        }
        public CommonFrame(List<CommonFrame> nextTwoFrames)
        {
            NextTwoFrames = nextTwoFrames;
        }

        public CommonFrame(CommonFrame previousFrame, List<CommonFrame> nextTwoFrames)
        {
            NextTwoFrames = nextTwoFrames;
            PreviousFrame = previousFrame;
        }

        public void MarkScore(int score)
        {
            HandleFault(score);

            BowlingNumber ballScore = new BowlingNumber();
            ballScore.Value = score;
            BallScores.Add(ballScore);

            HandleFrameStatus();

        }

        public void HandleFrameStatus()
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
                //handle frame status on second ball
                if (BallScores.Count == 2)
                {
                    IsClosed = IsSpare(BallScores);
                    IsReadyForNextFrame = true;
                    IsReadyToScore = (IsPreviousFrameScored && !IsClosed);
                }
                //handle frame status on first ball
                else if (BallScores.Count == 1)
                {
                    bool isStrikeOnFault = (BallScores[0].IsFault() && LostPinsByFault == Constant.NUMBER_OF_PINS);
                    IsClosed = (BallScores[0].IsStrike());
                    IsReadyForNextFrame = (BallScores[0].IsStrike() || isStrikeOnFault);
                    IsReadyToScore = (IsPreviousFrameScored && isStrikeOnFault) || (IsPreviousFrameScored && IsClosed && NextTwoBallsBowled());
//                    IsReadyToScore = IsPreviousFrameScored && BallScores[0].IsStrike() && NextTwoBallsBowled();
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

        protected bool NextTwoBallsBowled()
        {
                //if there is only one more frame
                if (this.NextTwoFrames.Count == 1)
                {
                    //Then the frame needs to have two balls bowled.
                    if (null != this.NextTwoFrames[0].BallScores[0] && this.NextTwoFrames[0].BallScores.Count == 2 && null != this.NextTwoFrames[0].BallScores[1])
                    {
                        return true;
                    }
                }
                //otherwise there should be two frames or more left.
                else if (this.NextTwoFrames.Count == 2)
                {
                    //In thiscase the balls could be in the next frame or in the next two frames.
                    if (this.NextTwoFrames[0].BallScores.Count == 2 && null != this.NextTwoFrames[0].BallScores[0] && null != this.NextTwoFrames[0].BallScores[1])
                    {
                        return true;
                    }
                    if (this.NextTwoFrames[0].BallScores.Count == 1 && this.NextTwoFrames[1].BallScores.Count == 1
                        && null != this.NextTwoFrames[0].BallScores[0] && null != this.NextTwoFrames[1].BallScores[0])
                    {
                        return true;
                    }
                }
            return false;
        }

        private void HandleFault(int score)
        {
            if (score < 0)
            {
                LostPinsByFault = Math.Abs(score);
            }
        }

        protected bool IsSpare(List<BowlingNumber> ballScores)
        {
            return (BallScores.Count == 2 && BowlingNumber.Sum(BallScores[0], BallScores[1]) == Constant.NUMBER_OF_PINS);
        }

        public int CalculateScore()
        {
            int score = (null != PreviousFrame) ? PreviousFrame.FrameScore : 0;
            if (IsReadyToScore)
            {
                //In this case the score is being calculated for a full frame of balls.
                if (BallScores.Count == CommonFrame.BallCount)
                {
                    score += BowlingNumber.Sum(BallScores[0], BallScores[1]);

                    //If the ball made a spare then the next ball also needs to be added to the ball.
                    if (IsSpare(BallScores))
                    {
                        //find the next ball
                        BowlingNumber nextBall = (null != NextTwoFrames && null != NextTwoFrames[0].BallScores) ? NextTwoFrames[0].BallScores[0] : null;
                        if (null != nextBall)
                        {
                            score += nextBall.Value;
                        }

                    }
                }
                else
                {
                    //In this case the frame is finished in the first frame.
                    ///<TODO>
                    /// Not required for this release however It could be interesting to check how this would work in a higher number of ballCount.
                    /// </TODO>
                    if (BallScores.Count == 1)
                    {
                        score += BallScores[0].Value;
                        if (BallScores[0].IsStrike())
                        {
                            //find the next two balls and add the scores.
                            BowlingNumber nextBall = (null != NextTwoFrames && null != NextTwoFrames[0].BallScores) ? NextTwoFrames[0].BallScores[0] : null;
                            score += null != nextBall ? nextBall.Value : 0;

                            if ((null != NextTwoFrames && (null != NextTwoFrames[0].BallScores || null != NextTwoFrames[1].BallScores)))
                            {
                                BowlingNumber nextNextBall = (NextTwoFrames[0].BallScores.Count == 1) ? NextTwoFrames[1].BallScores[0] : NextTwoFrames[0].BallScores[1];
                                score += nextNextBall.Value;
                            }
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

        public string FirstBallScore()
        {
            if (null != BallScores && BallScores.Count >0)
            {
                return BallScores[0].Text;
            }
            else
            {
                return "";
            }
        }

        public string SecondBallScore()
        {
            if (BallScores.Count >1 && null != BallScores)
            {
                return (IsSpare(BallScores)) ? "/" : BallScores[1].Text;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// This will clone a CommonFrame. The frame will have the next frames, previous frame and BallScores set back to null in the clone.
        /// </summary>
        /// <returns>clone of the CommonFrame with next frames, previous frame and BallScores set back to null.</returns>
        public object Clone()
        {
            //The cloning process is to create a copy of the frame and then to clear out the next frames, previous frame and ball scores so they can be created after cloning.
            CommonFrame newCommonFrame = (CommonFrame) this.MemberwiseClone();
            newCommonFrame.FrameScore = 0;
            newCommonFrame.IsClosed = false;
            newCommonFrame.IsReadyForNextFrame = false;
            newCommonFrame.IsReadyToScore  = false;
            newCommonFrame.IsPreviousFrameScored = false;
            newCommonFrame.LostPinsByFault = 0;
            newCommonFrame.NextTwoFrames = new List<CommonFrame>();
            newCommonFrame.PreviousFrame = null;
            newCommonFrame.BallScores = new List<BowlingNumber>();
            return newCommonFrame;
        }
    }
}
