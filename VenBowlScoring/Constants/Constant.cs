using System;
using System.Collections.Generic;
using System.Text;

namespace VenBowlScoring.Constants
{
    static class Constant
    {
        /// <summary>
        /// this constant defines the value used to mark a score as a fault. It needs to be outside of the range 0 to number of pins.
        /// </summary>
        public const int FAULT_VALUE = -1;

        /// <summary>
        /// This is the number of pins setup in the lane.
        /// </summary>
        public const int NUMBER_OF_PINS = 10;

        /// <summary>
        /// How many 3 ball frames are in the game. At the end of the game. 
        /// <TODO>
        /// This name is pretty bad and this could be used for some fun new game theories around bowling but for now this will do for the base game.
        /// </TODO>
        /// </summary>
        public const int DEFAULT_SPECIAL_FRAMES_PER_GAME = 1;

        /// <summary>
        /// How many 2 ball frames are in the game. at the start of the game.
        /// </summary>
        public const int DEFAULT_FRAMES_PER_GAME = 10;

        /// <summary>
        /// The first phase in the game creation process.
        /// </summary>
        public const string FIRST_GAME_PHASE = "Setup";

        public const string PLAY_GAME_PHASE = "Play";

    }
}
