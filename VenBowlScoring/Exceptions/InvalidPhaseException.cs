using System;
using System.Runtime.Serialization;

namespace VenBowlScoring.Exceptions
{
    [Serializable]
    public class InvalidPhaseException : Exception
    {
        public InvalidPhaseException()
        {
        }

        public InvalidPhaseException(string message) : base(message)
        {
        }

        public InvalidPhaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidPhaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}