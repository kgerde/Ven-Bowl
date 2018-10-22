using System;
using System.Collections.Generic;
using VenBowlScoring.Constants;
using VenBowlScoring.Exceptions;
using VenBowlScoring.Interface;

namespace VenBowlScoring.Model
{
    public class CommonFrame : ICommonFrame
    {
        public List<CommonFrame> NextTwoFrames;
        public CommonFrame PreviousFrame;


        public List<BowlingNumber> BallScores { get; private set; } = new List<BowlingNumber>();
        public int FrameScore { get; private set; }
        public bool IsClosed { get; private set; } = false;
        public bool IsReadyForNextFrame { get; private set; } = false;
        public bool IsReadyToScore { get; private set; } = false;
        public bool IsPreviousFrameScored { get; private set; } = false;

        public int LostPinsByFault { get; private set; }


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
            if (!IsPreviousFrameScored)
            {
                if (null != PreviousFrame)
                {
                    //Update the previous frame
                    PreviousFrame.HandleFrameStatus();
                }
                else
                {
                    IsPreviousFrameScored = true;
                }
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
                IsReadyToScore = (IsPreviousFrameScored && isStrikeOnFault);
            }
        }

        private void HandleFault(int score)
        {
            if (score < 0)
            {
                LostPinsByFault = Math.Abs(score);
            }
        }

        private bool IsSpare(List<BowlingNumber> ballScores)
        {
            return (BallScores.Count == 2 && BowlingNumber.Sum(BallScores[0], BallScores[1]) == Constant.NUMBER_OF_PINS);
        }

        public int CalculateScore()
        {
            int score = null != PreviousFrame ? PreviousFrame.FrameScore : 0;
            if (IsReadyToScore)
            {
                if (BallScores.Count == 2)
                {
                    score += BowlingNumber.Sum(BallScores[0], BallScores[1]);
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
                                BowlingNumber nextNextBall = (NextTwoFrames[0].BallScores[1] == null) ? NextTwoFrames[1].BallScores[1] : NextTwoFrames[0].BallScores[1];
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
            if (null != BallScores[0])
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
            if (null != BallScores[1])
            {
                return (IsSpare(BallScores)) ? "/" : BallScores[1].Text;
            }
            else
            {
                return "";
            }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
