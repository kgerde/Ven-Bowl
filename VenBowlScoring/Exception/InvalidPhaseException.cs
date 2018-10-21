using System;
using System.Runtime.Serialization;

namespace VenBowlScoring.Exception
{
    [Serializable]
    internal class InvalidPhaseException : Exception
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