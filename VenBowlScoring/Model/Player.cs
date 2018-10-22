using System.Collections.Generic;
using VenBowlScoring.Constants;
using VenBowlScoring.Interface;

namespace VenBowlScoring.Model
{
    /// <summary>
    /// A Player is a main object within the VenBowl library. It can be used to join a game.
    /// </summary>
    public class Player : IPlayer
    {
        #region properties

        /// <summary>
        /// Every player has to have a name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Players can be Bots or people. Bot or Person
        /// <TODO>
        ///     This should be a type instead of just text. to be updated later.
        /// </TODO>
        /// </summary>
        public string PlayerType { get; set; }

        /// <summary>
        /// Provides the player with an understanding of what games they are playing.
        /// </summary>
        public Game CurrentGame { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// Initialize a default player, or let the values be passed in to create a custom player.
        /// </summary>
        public Player(string name = "Player 1", string playerType = "Bot")
        {
            Name = name;
            PlayerType = playerType;
        }

        /// <summary>
        /// Returns all valid games matching the GameFilter passed in. 
        /// <TODO>
        /// For now there will be only one game.
        /// Game filter should work in a future release. for now it will just be unused.
        /// </TODO>
        /// </summary>
        /// <returns>the list of all games matching the GameFilter passed in.</returns>
        public List<string> FindGames(string gameFilter = Constant.FIRST_GAME_PHASE)
        {
            return new List<string> { new Game().Name };// ( (x) => x.currentPhase ==gameFilter ) };
        }

        /// <summary>
        /// Allow this player to create a game.
        /// <TODO>
        /// This should be implemented through the AddPlayers method.
        /// </TODO>
        /// </summary>
        /// <param name="gameName"></param>
        public void HostGame(string gameName)
        {
            CurrentGame = new Game();
            CurrentGame.Name = gameName;
            CurrentGame.JoinedPlayers.Add(this);
        }

        /// <summary>
        /// Allow this player to join another players game.
        /// <TODO>
        /// Not Implemented properly yet... This should go through the Game.JoinGame() method.
        /// </TODO>
        /// </summary>
        /// <param name="game"></param>
        public void JoinGame(Game game)
        {
            game.JoinedPlayers.Add(this);
            CurrentGame = game;
        }


        /// <summary>
        /// For now Review history will only print out the current game.
        /// <TODO>
        /// A later version can display previous games.
        /// </TODO>
        /// </summary>
        public string ReviewHistory()
        {
            return CurrentGame.Scorecard.Print(this);
        }

        /// <summary>
        /// Simulate a basic Bot.
        /// </summary>
        /// <param name="score">int default to 10. The number of pins nocked down by a ball.</param>
        /// <returns>the score as an int.</returns>
        public int TakeTurn(int score = 10)
        {
            //take 2 seconds and then get a perfect score for now.
            //            Thread.Sleep(2000);
            return score;
        }
        #endregion

    }
}