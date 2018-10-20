using System;
using System.Collections.Generic;
using System.Text;
using VenBowlScoring.Interface;

namespace VenBowlScoring.Model
{
    class Turn : ITurn
    {
        public IPlayer CurrentPlayer;
        public IScorableFrame CurrentFrameSet;
        public void Play()
        {
            throw new NotImplementedException();
        }

        public void Score()
        {
            throw new NotImplementedException();
        }
    }
}
