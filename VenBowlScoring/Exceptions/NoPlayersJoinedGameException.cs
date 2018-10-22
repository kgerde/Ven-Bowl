using System;
using System.Runtime.Serialization;

namespace VenBowlScoring.Exceptions
{
    [Serializable]
    public class NoPlayersJoinedGameException : Exception
    {
        public NoPlayersJoinedGameException()
        {
        }

        public NoPlayersJoinedGameException(string message) : base(message)
        {
        }

        public NoPlayersJoinedGameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoPlayersJoinedGameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}