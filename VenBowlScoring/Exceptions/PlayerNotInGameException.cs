using System;
using System.Runtime.Serialization;

namespace VenBowlScoring.Exceptions
{
    [Serializable]
    public class PlayerNotInGameException : Exception
    {
        public PlayerNotInGameException()
        {
        }

        public PlayerNotInGameException(string message) : base(message)
        {
        }

        public PlayerNotInGameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PlayerNotInGameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}