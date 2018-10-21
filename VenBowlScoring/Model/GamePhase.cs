using System.Collections.Generic;

namespace VenBowlScoring.Model
{
    /// <summary>
    /// This object is used by a game to provide players time to join the game/play/review and celebrate the game/clean up after the game.
    /// </summary>
    public class GamePhase
    {
        /// <summary>
        /// parameter to hold the current game phase.
        /// </summary>
        public string CurrentPhase { get; private set; }

        /// <summary>
        /// All games have 4 phases of actions: 
        ///     setup includes waiting for players and preparing the game before play can start, 
        ///     play includes all play activities, 
        ///     review includes celebration and score reprots, 
        ///     clean up includes anything that has to be done after a game to leave a game.
        ///     <TODO>
        ///         Once I have configurations working I would move this to a configuration file. but for now this will be fine.
        ///     </TODO>
        /// </summary>
        public static List<string> Phases = new List<string> {"Setup","Play","Review","Clean up"};

        /// <summary>
        /// Initialize the Game Phases and set the initial phase as the current phase.
        /// </summary>
        GamePhase ()
        {
            CurrentPhase = Phases[0];
        }

        /// <summary>
        /// change the phase to the next phase.
        /// </summary>
        void ChangePhase()
        {
            CurrentPhase = Phases[Phases.IndexOf(CurrentPhase) + 1];
        }
    }
}