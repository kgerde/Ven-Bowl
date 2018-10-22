using System;
using System.Runtime.Serialization;

namespace VenBowlScoring.Exceptions
{
    [Serializable]
    public class GameNotStartedException : Exception
    {
        public GameNotStartedException()
        {
        }

        public GameNotStartedException(string message) : base(message)
        {
        }

        public GameNotStartedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GameNotStartedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}