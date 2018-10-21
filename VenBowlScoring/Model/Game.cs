using System;
using System.Collections.Generic;
using System.Text;
using VenBowlScoring.Interface;
using VenBowlScoring.Exceptions;

namespace VenBowlScoring.Model
{
    /// <summary>
    /// This class is the main object for bowling a game. Players may join a game to play.
    /// </summary>
    class Game : IGame
    {
        public List<Player> JoinedPlayers;
        public string Name = "Default Game";

        /// <summary>
        /// The following two fields could be encapsolated within a GameTurn object.
        /// </summary>
        public ITurn CurrentTurn;
        //        public IPlayer CurrentPlayer;
        //        public IScorableFrame CurrentFrameSet;

            /// <TODO>
            /// I believe the game does not care about frames per game. that may be something that only the score card cares about... the game most likely only cares about the number of turns per game.
            /// </TODO>
        private int FramesPerGame = 10;
        public DateTime GameTime;
        public float Duration;
        public GamePhase CurrentPhase;
        public Scorecard Scorecard = new Scorecard();


        /// <summary>
        /// Game sets up a Default Game.
        /// </summary>
        public Game()
        {
            JoinedPlayers = new List<Player> { };
        }
        public void AddPlayers(IEnumerable<Player> players)
        {
            JoinedPlayers.AddRange(players);
        }

        public void FinishGame()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
        }

        public void Play(IBowlAttempt nextAttempt)
        {
            throw new NotImplementedException();
        }

        public void RemovePlayers(IEnumerable<IPlayer> players)
        {
            throw new NotImplementedException();
        }

        public void StartGame()
        {
            //ensure the game has players
            if (null != JoinedPlayers && JoinedPlayers.Count > 0)
            {
                //change the game phase to start the game.
                CurrentPhase.ChangePhase();
                //create the score card with all frames set to defaults
                this.CreateScoreCard();
            }
            else
            {
                throw new NoPlayersJoinedGameException("No players have joined the game. A game must have at least 1 player to start.");
            }
        }

        private void CreateScoreCard()
        {

        }

        public void RemovePlayers(IEnumerable<Player> players)
        {
            throw new NotImplementedException();
        }
    }
}
