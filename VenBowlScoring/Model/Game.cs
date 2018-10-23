using System;
using System.Collections.Generic;
using VenBowlScoring.Constants;
using VenBowlScoring.Exceptions;
using VenBowlScoring.Interface;


namespace VenBowlScoring.Model
{
    /// <summary>
    /// This class is the main object for bowling a game. Players may join a game to play.
    /// </summary>
    public class Game : IGame
    {
        #region properties

        /// <summary>
        /// List of all players ready to start the game or in the game.
        /// </summary>
        public List<Player> JoinedPlayers { get; set; }

        /// <summary>
        /// Name of the current game.
        /// </summary>
        public string Name { get; set; } = "Default Game";

        /// <TODO>
        /// changing the game over to a task factory of turns would most likely be a much better design now that I have seen the whole system.
        /// </TODO>

        /// <summary>
        /// How many frames will each game have?
        /// </summary>
        public int FramesPerGame { get; set; } = Constant.DEFAULT_FRAMES_PER_GAME;

        /// <summary>
        /// How many final frames will each game have?
        /// </summary>
        public int SpecialFramesPerGame { get; set; } = Constant.DEFAULT_SPECIAL_FRAMES_PER_GAME;

        /// <summary>
        /// starting date and time of the game.
        /// </summary>
        public DateTime GameTime { get; private set; }

        /// <summary>
        /// Duration of the game as a time span.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// The current phase of the game as a Game Phase object.
        /// </summary>
        public GamePhase CurrentPhase { get; private set; }

        /// <summary>
        /// The scorecard for the game. This object is very simialar to what you would find in a bowing allow for providing the end user with a score.
        /// </summary>
        public Scorecard Scorecard { get; private set; }

        #endregion

        #region Methods
        /// <summary>
        /// Game sets up a Default Game.
        /// </summary>
        public Game()
        {
            JoinedPlayers = new List<Player> { };
            CurrentPhase = new GamePhase();
        }

        /// <summary>
        /// Add a list of players to the current game.
        /// </summary>
        /// <param name="players">IEnumerable of Players to add to the game.</param>
        /// <exception cref="ActiveGameException">ActiveGameException can be thrown if the game is already started.</exception>
        public void AddPlayers(IEnumerable<Player> players)
        {
            if (CurrentPhase.CurrentPhase == Constant.FIRST_GAME_PHASE)
            {
                JoinedPlayers.AddRange(players);
            }
            else
            {
                throw new ActiveGameException("The game has already been started. Players can only be added to games before they are started.");
            }
        }

        /// <summary>
        /// Final Step of a game.
        /// </summary>
        public void FinishGame()
        {
            CurrentPhase.ChangePhase(Constant.FINAL_GAME_PHASE);
            Duration = (DateTime.UtcNow).Subtract(GameTime);
        }

        /// <summary>
        /// This method allows all joined bowlers to take their turns until the game is over and scores are ready to be calculated.
        /// </summary>
        /// <exception cref="GameNotStartedException">GameNotStartedException can be thrown if a game has not yet been started but play is attempted.</exception>
        public void Play()
        {
            if (CurrentPhase.CurrentPhase == Constant.PLAY_GAME_PHASE)
            {
                try
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
                                if (ballScore < Constant.NUMBER_OF_PINS)
                                {
                                    finalFrameLessThanPins += ballScore;
                                    if (finalFrameLessThanPins < Constant.NUMBER_OF_PINS && finalBalls == 1)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        //Lets have the bowlers display their progress as part of the game as well, after the turn is taken.
                        foreach (Player bowler in JoinedPlayers)
                        {
                            //To ensure all frames were scored. The frames need to have their scores calculated one last time.
                            ((FinalFrame)Scorecard.Sheet[bowler][bowler.CurrentGame.Scorecard.Sheet[bowler].Count - 1]).HandleFrameStatus();

                            bowler.ReviewHistory();
                        }

                    }
                }
                finally
                {
                    FinishGame();
                }
            }
            else
            {
                throw new GameNotStartedException("The game has not yet been started. Please start the game before play can begin.");
            }
        }

        /// <summary>
        /// Remove a list of players from the game.
        /// </summary>
        /// <param name="players">IEnumerable Players list to be removed from the game.</param>
        public void RemovePlayers(IEnumerable<Player> players)
        {
            if (CurrentPhase.CurrentPhase == Constant.FIRST_GAME_PHASE)
            {
                if (null == JoinedPlayers)
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
                catch (Exception)
                {
                    throw new PlayerNotInGameException("The player could not be Removed from the Game. The player was not joined.");
                }
            }
            else
            {
                throw new ActiveGameException("The game is active. Players may not be removed from the game once it has started.");
            }
        }

        /// <summary>
        /// Starts a game.
        /// </summary>
        /// <exception cref="NoPlayersJoinedGameException" >NoPlayersJoinedGameException thrown when a game is started without at least 1 player.</exception>
        public void StartGame()
        {
            //ensure the game has players
            if (null != JoinedPlayers && JoinedPlayers.Count > 0)
            {
                GameTime = DateTime.UtcNow;
                //change the game phase to start the game.
                CurrentPhase.ChangePhase();
                //create the score card with all frames set to defaults
                CreateScoreCard();
                //start play automaticly
                Play();
            }
            else
            {
                throw new NoPlayersJoinedGameException("No players have joined the game. A game must have at least 1 player to start.");
            }
        }

        /// <summary>
        /// Starts a game. and pass along scores to be used.
        /// </summary>
        /// <exception cref="NoPlayersJoinedGameException" >NoPlayersJoinedGameException thrown when a game is started without at least 1 player.</exception>
        public void StartGame(List<int>ballScores)
        {
            //ensure the game has players
            if (null != JoinedPlayers && JoinedPlayers.Count > 0)
            {
                GameTime = DateTime.UtcNow;
                //change the game phase to start the game.
                CurrentPhase.ChangePhase();
                //create the score card with all frames set to defaults
                CreateScoreCard();
                //start play automaticly
                Play(ballScores);
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
            Scorecard = new Scorecard(JoinedPlayers, FramesPerGame, SpecialFramesPerGame);
        }

        #endregion

        /// <summary>
        /// This method allows all joined bowlers to take their turns until the game is over and scores are ready to be calculated.
        /// </summary>
        /// <exception cref="GameNotStartedException">GameNotStartedException can be thrown if a game has not yet been started but play is attempted.</exception>
        public void Play(List<int> ballScores)
        {
            if (CurrentPhase.CurrentPhase == Constant.PLAY_GAME_PHASE)
            {
                try
                {
                    //Ensure this queue has plenty of data to dequeue.
                    while ((ballScores.Count) < (Constant.DEFAULT_FRAMES_PER_GAME * Constant.DEFAULT_FRAMES_BALLS) + (Constant.DEFAULT_SPECIAL_FRAMES_PER_GAME * Constant.DEFAULT_SPECIAL_FRAMES_BALLS))
                    {
                        ballScores.AddRange(ballScores);
                    }
                    Queue<int> balls = new Queue<int>(ballScores);
                    //ensure the game plays all frames.
                    for (int frame = SpecialFramesPerGame; frame < FramesPerGame; frame++)
                    {
                        //for now we will have each bowler take their turn and let the bowler control the score recieved in the TakeTurn method.
                        foreach (Player bowler in JoinedPlayers)
                        {
                            ///<TODO>
                            ///Redesign this section of the game to have a bowler complete a frame instead of just taking a turn.
                            ///</TODO>
                            int ballScore = bowler.TakeTurn(balls.Dequeue());
                            Scorecard.MarkScore(bowler, ballScore);

                            //Did the player get a strike or all pin fault? If so the next player goes otherwise the bowler gets the second turn.
                            CommonFrame checkForSecondBall = new CommonFrame();
                            checkForSecondBall.MarkScore(ballScore);
                            if (!checkForSecondBall.IsReadyForNextFrame)
                            {
                                Scorecard.MarkScore(bowler, bowler.TakeTurn(balls.Dequeue()));
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
                            int finalFrameOpenCounter = 0;
                            for (int finalBalls = 0; finalBalls < FinalFrame.BallCount; finalBalls++)
                            {
                                ///<TODO>
                                ///Redesign this section of the game to have a bowler complete a frame instead of just taking a turn.
                                ///</TODO>
                                int ballScore = bowler.TakeTurn(balls.Dequeue());
                                Scorecard.MarkScore(bowler, ballScore);

                                finalFrameOpenCounter += ballScore;
                                //Break out of the loop if The first two balls are an open frame.
                                if (finalFrameOpenCounter < Constant.NUMBER_OF_PINS && finalBalls == 1)
                                {
                                    break;
                                }
                            }
                        }

                        //Lets have the bowlers display their progress as part of the game as well, after the turn is taken.
                        foreach (Player bowler in JoinedPlayers)
                        {
                            //To ensure all frames were scored. The frames need to have their scores calculated one last time.
                            ((FinalFrame)Scorecard.Sheet[bowler][bowler.CurrentGame.Scorecard.Sheet[bowler].Count - 1]).HandleFrameStatus();

                            bowler.ReviewHistory();
                        }

                    }
                }
                finally
                {
                    FinishGame();
                }
            }
            else
            {
                throw new GameNotStartedException("The game has not yet been started. Please start the game before play can begin.");
            }
        }

    }
}
