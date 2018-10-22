using System;
using System.Collections.Generic;
using VenBowlScoring.Constants;
using VenBowlScoring.Exceptions;

namespace VenBowlScoring.Model
{
    /// <summary>
    /// This object is used by a game to provide players time to join the game/play/review and celebrate the game/clean up after the game.
    /// </summary>
    public class GamePhase
    {

        #region properties
        /// <summary>
        /// property to hold the current game phase.
        /// </summary>
        public string CurrentPhase { get; private set; }

        /// <summary>
        /// All games have 4 phases of actions: 
        ///     setup includes waiting for players and preparing the game before play can start, 
        ///     play includes all play activities, 
        ///     review includes celebration and score reprots, 
        ///     clean up includes anything that has to be done after a game to leave a game,
        ///     completed is the final phase.
        ///     <TODO>
        ///         Once I have configurations working I would move this to a configuration file. but for now this will be fine.
        ///     </TODO>
        /// </summary>
        public static List<string> Phases = new List<string> {Constant.FIRST_GAME_PHASE,Constant.PLAY_GAME_PHASE, Constant.REVIEW_GAME_PHASE
            ,Constant.CLEAN_UP_GAME_PHASE,Constant.FINAL_GAME_PHASE};
        #endregion

        #region Methods

        /// <summary>
        /// Initialize the Game Phases and set the initial phase as the current phase.
        /// </summary>
        public GamePhase()
        {
            CurrentPhase = Phases[0];
        }

        /// <summary>
        /// change the phase to the next phase.
        /// </summary>
        /// <exception cref="InvalidPhaseException">This function will throw an invalid phase exception if
        ///     No phase is passed in and the game is Completed or
        ///     A valid phase is not passed in.</exception>
        public void ChangePhase(string phases = null)
        {
            if (null != phases || Phases.Contains(phases))
            {
                //Set the phase as the one passed in.
                CurrentPhase = Phases[Phases.IndexOf(phases)];
            }
            else
            {
                if (null == phases)
                {

                    try
                    {
                        //the next phase is the index of the current phase + 1.
                        CurrentPhase = Phases[Phases.IndexOf(CurrentPhase) + 1];
                    }
                    catch (Exception)
                    {
                        throw new InvalidPhaseException("This game is Completed. The Phase cannot be updated further.");
                    }
                }
                else
                {
                    throw new InvalidPhaseException("The game phase requested does not exist. The Phase cannot be updated.");
                }
            }
        }
        #endregion

    }
}