using System;
using System.Collections.Generic;
using System.Text;
using VenBowlScoring.Interface;

namespace VenBowlScoring.Model
{
    /// <summary>
    /// This class is the main object for bowling a game. Players may join a game to play.
    /// </summary>
    class Game : IGame
    {
        public List<Player> JoinedPlayers;

        /// <summary>
        /// The following two fields could be encapsolated within a GameTurn object.
        /// </summary>
        public ITurn CurrentTurn;
        //        public IPlayer CurrentPlayer;
        //        public IScorableFrame CurrentFrameSet;
        private int FramesPerGame = 10;
        public DateTime GameTime;
        public float Duration;
        public GamePhase CurrentPhase;


    /// <summary>
    /// Game sets up a Default Game.
    /// </summary>
    void Game()
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
            if (null != JoinedPlayers && JoinedPlayers.Count > 0)
            {
                CurrentFrameSet = JoinedPlayers[0].CurrentGameFrames[0];
            }
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
            throw new NotImplementedException();
        }

        public void RemovePlayers(IEnumerable<Player> players)
        {
            throw new NotImplementedException();
        }
    }
}
