using System;
using System.Runtime.Serialization;

namespace VenBowlScoring.Exceptions
{
    [Serializable]
    internal class ActiveGameException : Exception
    {
        public ActiveGameException()
        {
        }

        public ActiveGameException(string message) : base(message)
        {
        }

        public ActiveGameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ActiveGameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}