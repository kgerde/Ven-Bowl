using System;
using System.Collections.Generic;
using System.Text;
using VenBowlScoring.Interface;

namespace VenBowlScoring.Model
{
    /// <summary>
    /// A Player is a main object within the VenBowl library. It can be used to join a game.
    /// </summary>
    public class Player : IPlayer
    {
        public string Name { get; set; }
       
        public List<IScorableFrame> CurrentGameFrames = ;

        /// <summary>
        /// 
        /// </summary>
        Player()
        {
            Name = "Player 1";
            SetupGameFrames();
        }

        private void SetupGameFrames()
        {
            CurrentGameFrames = new List<IScorableFrame>;
            for (int normalFrames = 0; normalFrames < V; normalFlames++)
            {
                CurrentGameFrames.Add(new CommonFrame());
            }
        }

        public void ReviewHistory()
        {
            throw new NotImplementedException();
        }

        public void TakeTurn()
        {
            throw new NotImplementedException();
        }
    }
}