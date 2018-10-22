using System;
using System.Collections.Generic;
using System.Text;
using VenBowlScoring.Interface;
using VenBowlScoring.Exceptions;
using VenBowlScoring.Constants;


namespace VenBowlScoring.Model
{
    /// <summary>
    /// This class is the main object for bowling a game. Players may join a game to play.
    /// </summary>
    public class Game : IGame
    {
        public List<Player> JoinedPlayers { get; set; }
        public string Name { get; set; } = "Default Game";

        /// <summary>
        /// The following two fields could be encapsolated within a GameTurn object.
        /// </summary>
//        public ITurn CurrentTurn { get; set; }
        //        public IPlayer CurrentPlayer;
        //        public IScorableFrame CurrentFrameSet;

        /// <TODO>
        /// I believe the game does not care about frames per game. that may be something that only the score card cares about... the game most likely only cares about the number of turns per game.
        /// </TODO>
        public int FramesPerGame { get; set; } = Constant.DEFAULT_FRAMES_PER_GAME;
        public int SpecialFramesPerGame { get; set; } = Constant.DEFAULT_SPECIAL_FRAMES_PER_GAME;
        public DateTime GameTime { get; private set; }
        public TimeSpan Duration { get; private set; }
        public GamePhase CurrentPhase { get; private set; }
        public Scorecard Scorecard { get; private set; }


        /// <summary>
        /// Game sets up a Default Game.
        /// </summary>
        public Game()
        {
            JoinedPlayers = new List<Player> { };
            CurrentPhase = new GamePhase();
        }
        public void AddPlayers(IEnumerable<Player> players)
        {
            JoinedPlayers.AddRange(players);
        }

        public void FinishGame()
        {
            Duration = (DateTime.UtcNow).Subtract(this.GameTime);
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method allows all joined bowlers to take their turns until the game is over and scores are ready to be calculated.
        /// </summary>
        public void Play()
        {
            if (this.CurrentPhase.CurrentPhase == Constant.PLAY_GAME_PHASE)
            {

                //ensure the game plays all frames.
                for (int frame = SpecialFramesPerGame; frame < FramesPerGame; frame++)
                {
                    //for now we will have each bowler take their turn and let the bowler control the score recieved in the TakeTurn method.
                    foreach (Player bowler in JoinedPlayers)
                    {
                        ///<TODO>
                        ///Redesign this section of the game to have a bowler complete a frame instead of just taking a turn.
                        ///</TODO>
                        int ballScore = bowler.TakeTurn();
                        Scorecard.MarkScore(bowler, ballScore);

                        //Did the player get a strike or all pin fault? If so the next player goes otherwise the bowler gets the second turn.
                        CommonFrame checkForSecondBall = new CommonFrame();
                        checkForSecondBall.MarkScore(ballScore);
                        if (!checkForSecondBall.IsReadyForNextFrame)
                        {
                            Scorecard.MarkScore(bowler, bowler.TakeTurn());
                        }
                    }

                    //Lets have the bowlers display their progress as part of the game as well, after the turn is taken.
                    foreach (Player bowler in JoinedPlayers)
                    {
                        bowler.ReviewHistory();
                    }
                }

                //run the final frames
                for (int frame = 0; frame < SpecialFramesPerGame; frame++)
                {
                    //for now we will have each bowler take their turn and let the bowler control the score recieved in the TakeTurn method.
                    foreach (Player bowler in JoinedPlayers)
                    {
                        int finalFrameLessThanPins = 0;
                        for (int finalBalls = 0; finalBalls < FinalFrame.BallCount; finalBalls++)
                        {
                            ///<TODO>
                            ///Redesign this section of the game to have a bowler complete a frame instead of just taking a turn.
                            ///</TODO>
                            int ballScore = bowler.TakeTurn();
                            Scorecard.MarkScore(bowler, ballScore);

                            ///<TODO>
                            ///This code needs to be cleaned up and put into a task. My design didn't quite work out for making this code clean.
                            /// </TODO>
                            if(ballScore < Constant.NUMBER_OF_PINS)
                            {
                                finalFrameLessThanPins+= ballScore;
                                if(finalFrameLessThanPins< Constant.NUMBER_OF_PINS && finalBalls==1)
                                {
                                    break;
                                }
                            }
                        }

                        //Did the player get a strike or all pin fault? If so the next player goes otherwise the bowler gets the second turn.
                    }

                    //Lets have the bowlers display their progress as part of the game as well, after the turn is taken.
                    foreach (Player bowler in JoinedPlayers)
                    {
                        bowler.ReviewHistory();
                    }

                }



            }
            else
            {
                throw new GameNotStartedException("The game has not yet been started. Please start the game before play can begin.");
            }
        }

        public void RemovePlayers(IEnumerable<Player> players)
        {
            if(null == JoinedPlayers)
            {
                throw new PlayerNotInGameException("The player could not be Removed from the Game. No players are joined.");
            }
            try
            {
                foreach (Player player in players)
                {
                    JoinedPlayers.Remove(player);
                }
            }
            catch(Exception)
            { 
                throw new PlayerNotInGameException("The player could not be Removed from the Game. The player was not joined.");
            }
        }

        public void StartGame()
        {
            //ensure the game has players
            if (null != JoinedPlayers && JoinedPlayers.Count > 0)
            {
                this.GameTime = DateTime.UtcNow;
                //change the game phase to start the game.
                CurrentPhase.ChangePhase();
                //create the score card with all frames set to defaults
                this.CreateScoreCard();
                //start play automaticly
                this.Play();
            }
            else
            {
                throw new NoPlayersJoinedGameException("No players have joined the game. A game must have at least 1 player to start.");
            }
        }

        /// <summary>
        /// Creates a score card based on the players in the game and the frames configuration values.
        /// </summary>
        private void CreateScoreCard()
        {
            this.Scorecard = new Scorecard(JoinedPlayers, this.FramesPerGame, this.SpecialFramesPerGame);
        }
    }
}
