using System;
using System.Collections.Generic;
using VenBowlScoring.Constants;
using VenBowlScoring.Exceptions;
using VenBowlScoring.Interface;

namespace VenBowlScoring.Model
{
    /// <summary>
    /// Can be used to create a standard bowling frame.
    /// </summary>
    public class CommonFrame : ICommonFrame
    {

        #region properties

        /// <summary>
        /// How many balls per frame. by default this is 2.
        /// </summary>
        public static int BallCount { get; internal set; } = Constant.DEFAULT_FRAMES_BALLS;

        /// <summary>
        /// Strikes are calculated differently in the final frame. When set to true this property will change the frame logic to that of a final frame.
        /// <TODO>
        /// Not implemented.
        /// </TODO>
        /// </summary>
        public static bool StrikeContinueLogic = false;

        /// <summary>
        /// Spares are calculated differently in the final frame. When set to true this property will change the frame logic to that of a final frame.
        /// <TODO>
        /// Not implemented.
        /// </TODO>
        /// </summary>
        public static bool SpareContinueLogic = false;

        /// <summary>
        /// pointers to the next two frames to allow scores to be calculated as strikes require the next two balls.
        /// <TODO>
        /// redesign this code to be ball oriented instead of frame oriented. This will drasticly simplify the code.
        /// </TODO>
        /// </summary>
        public List<CommonFrame> NextTwoFrames = new List<CommonFrame>();

        /// <summary>
        /// pointer to the previous frame so that previous scored frames or frames that need to be scored can be accessed.
        /// </summary>
        public CommonFrame PreviousFrame;


        /// <summary>
        /// Scores for each ball of the frame are stored as Bowling Numbers so that they can be easily added and printed.
        /// </summary>
        public List<BowlingNumber> BallScores { get; protected set; } = new List<BowlingNumber>();

        /// <summary>
        /// The Score for this frame. This value is only set when IsScoringComplete is set to true.
        /// </summary>
        public int FrameScore { get; protected set; }

        /// <summary>
        /// Is the frame closed or opened.
        /// </summary>
        public bool IsClosed { get; protected set; } = false;

        /// <summary>
        /// Is the frame ready to have the next frame processed so that strikes and spares can be calculated once the next frame is started.
        /// </summary>
        public bool IsReadyForNextFrame { get; protected set; } = false;

        /// <summary>
        /// Is the frame ready to be scored. Strikes and Spares require special rules around this.
        /// </summary>
        public bool IsReadyToScore { get; protected set; } = false;

        /// <summary>
        /// Is the previous frame scored. This is one of the needed information to define if the frame is ready to be scored.
        /// </summary>
        public bool IsPreviousFrameScored { get; protected set; } = false;

        /// <summary>
        /// How many pins were hit during a line fault for this frame. Any pins nocked down during a line fault are lost from the game's possible score.
        /// </summary>
        public int LostPinsByFault { get; protected set; }

        /// <summary>
        /// Is the scoring process completed for this frame?
        /// </summary>
        public bool IsScoringComplete { get; protected set; }

        #endregion

        #region Methods

        #region Constructors

        /// <summary>
        /// default constructor. If this constructur is used previousFrame and nextTwoFrames must be manually handled through the properties.
        /// </summary>
        public CommonFrame()
        {
        }

        /// <summary>
        /// constructor allowing previousFrame to be passed in. Useful for the last frame. nextTwoFrames has to be managed through the property.
        /// </summary>
        public CommonFrame(CommonFrame previousFrame)
        {
            PreviousFrame = previousFrame;
        }

        /// <summary>
        /// constructor allowing the nextTwoFrames to be passed in. Useful for the first frame. previousFrame has to be managed through the property.
        /// </summary>
        /// <param name="nextTwoFrames">The list of the next two frames in the game so that the next two balls can be found for strikes.</param>
        public CommonFrame(List<CommonFrame> nextTwoFrames)
        {
            NextTwoFrames = nextTwoFrames;
        }

        /// <summary>
        /// Preffered constructor. Takes in both previous frame and next two frames to provide the class with all required objects.
        /// </summary>
        /// <param name="previousFrame">The previous frame in the game.</param>
        /// <param name="nextTwoFrames">The next two frames in the game.</param>
        public CommonFrame(CommonFrame previousFrame, List<CommonFrame> nextTwoFrames)
        {
            NextTwoFrames = nextTwoFrames;
            PreviousFrame = previousFrame;
        }

        #endregion


        /// <summary>
        /// record a ball score in the frame.
        /// </summary>
        /// <param name="score">score of the ball between -X and X. where X is the number of possible pins. negative numbers allow for faults to be recorded.</param>
        public virtual void MarkScore(int score)
        {
            HandleFault(score);

            BowlingNumber ballScore = new BowlingNumber();
            ballScore.Value = score;
            BallScores.Add(ballScore);

            HandleFrameStatus();

        }

        /// <summary>
        /// The state engine controller. this method sets all of the boolean values to allow for description of the frame's game state.
        /// </summary>
        public virtual void HandleFrameStatus()
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
                    IsReadyToScore = (IsPreviousFrameScored && !IsClosed) || ((IsPreviousFrameScored && IsClosed && this.NextTwoFrames.Count > 0 && this.NextTwoFrames[0].BallScores.Count > 0)) || ((IsPreviousFrameScored && IsClosed && this.NextTwoFrames.Count == 0));
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

        /// <summary>
        /// Confirm the next two balls have been bowled. Used by the strike scoring calculations.
        /// </summary>
        /// <returns>True if the next two balls have been bowled, otherwise false.</returns>
        protected virtual bool NextTwoBallsBowled()
        {
            //if there is only one more frame
            if (NextTwoFrames.Count == 1)
            {
                //Then the frame needs to have two balls bowled.
                if (null != NextTwoFrames[0].BallScores[0] && NextTwoFrames[0].BallScores.Count == 2 && null != NextTwoFrames[0].BallScores[1])
                {
                    return true;
                }
            }
            //otherwise there should be two frames or more left.
            else if (NextTwoFrames.Count == 2)
            {
                //In thiscase the balls could be in the next frame or in the next two frames.
                if (NextTwoFrames[0].BallScores.Count == 2 && null != NextTwoFrames[0].BallScores[0] && null != NextTwoFrames[0].BallScores[1])
                {
                    return true;
                }
                if (NextTwoFrames[0].BallScores.Count == 1 && NextTwoFrames[1].BallScores.Count == 1
                    && null != NextTwoFrames[0].BallScores[0] && null != NextTwoFrames[1].BallScores[0])
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// deal with the extra work required for a fault to be handled.
        /// </summary>
        /// <param name="score">Integer expected to be between -X and -1 where X is the number of pins being played.</param>
        protected void HandleFault(int score)
        {
            if (score < 0)
            {
                LostPinsByFault = Math.Abs(score);
            }
        }

        /// <summary>
        /// Confirm if a frame is a spare.
        /// </summary>
        /// <param name="ballScores">The BowlingNumber list of scores for the frame.</param>
        /// <returns>True when the balls scores are a spare, otherwise false.</returns>
        public bool IsSpare(List<BowlingNumber> ballScores)
        {
            return (BallScores.Count == 2 && BowlingNumber.Sum(BallScores[0], BallScores[1]) == Constant.NUMBER_OF_PINS);
        }

        /// <summary>
        /// The method to calculate the score of a frame.
        /// </summary>
        /// <returns>integer Score of the frame</returns>
        /// <exception cref="FrameNotReadyToScoreException">FrameNotReadyToScoreException</exception>
        public virtual int CalculateScore()
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


        /// <summary>
        /// Method allowing the ball of the frame to be pretty printed.
        /// </summary>
        /// <returns>Pretty print view of the first ball score of a frame.</returns>
        public string BallScoreText(int ballNumber)
        {
            if (null != BallScores && BallScores.Count > ballNumber - 1)
            {
                return BallScores[ballNumber - 1].Text;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Method allowing the first ball of the frame to be pretty printed.
        /// </summary>
        /// <returns>Pretty print view of the first ball score of a frame.</returns>
        public string FirstBallScore()
        {
            if (null != BallScores && BallScores.Count > 0)
            {
                return BallScores[0].Text;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Method allowing the second ball of the frame to be pretty printed.
        /// </summary>
        /// <returns>Pretty print view of the second ball score of a frame.</returns>
        public string SecondBallScore()
        {
            if (BallScores.Count > 1 && null != BallScores)
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
            CommonFrame newCommonFrame = (CommonFrame)MemberwiseClone();
            newCommonFrame.FrameScore = 0;
            newCommonFrame.IsClosed = false;
            newCommonFrame.IsReadyForNextFrame = false;
            newCommonFrame.IsReadyToScore = false;
            newCommonFrame.IsPreviousFrameScored = false;
            newCommonFrame.LostPinsByFault = 0;
            newCommonFrame.NextTwoFrames = new List<CommonFrame>();
            newCommonFrame.PreviousFrame = null;
            newCommonFrame.BallScores = new List<BowlingNumber>();
            return newCommonFrame;
        }
        #endregion

    }
}
