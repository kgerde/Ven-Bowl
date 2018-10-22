using System;
using System.Runtime.Serialization;

namespace VenBowlScoring.Exceptions
{
    [Serializable]
    public class InvalidBallScoreException : Exception
    {
        public InvalidBallScoreException()
        {
        }

        public InvalidBallScoreException(string message) : base(message)
        {
        }

        public InvalidBallScoreException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidBallScoreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}