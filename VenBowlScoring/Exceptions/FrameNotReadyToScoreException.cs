using System;
using System.Runtime.Serialization;

namespace VenBowlScoring.Exceptions
{
    [Serializable]
    public class FrameNotReadyToScoreException : Exception
    {
        public FrameNotReadyToScoreException()
        {
        }

        public FrameNotReadyToScoreException(string message) : base(message)
        {
        }

        public FrameNotReadyToScoreException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FrameNotReadyToScoreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}