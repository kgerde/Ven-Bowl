using System;
using System.Collections.Generic;
using System.Text;
using VenBowlScoring.Interface;
using VenBowlScoring.Constants;

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

        #endregion

        #region methods

        /// <summary>
        /// Initialize a default player, or let the values be passed in to create a custom player.
        /// </summary>
        Player(string name = "Player 1", string playerType = "Bot")
        {
            Name = name;
            PlayerType = playerType;
            SetupGameFrames();
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

        public void JoinGame()
        {

        }



        private void SetupGameFrames()
        {
            ///<todo>the following section is most likely best in the games area. not sure player needs to know about the frames...</todo>
 /*           CurrentGameFrames = new List<IScorableFrame>;
            for (int normalFrames = 0; normalFrames < V; normalFlames++)
            {
                CurrentGameFrames.Add(new CommonFrame());
            }
*/
        }

        public void ReviewHistory()
        {
            throw new NotImplementedException();
        }

        public void TakeTurn()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}