using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SSD
{
    [ExcludeFromCodeCoverage] // NOTE: This class does not implement custom logic.  Remove this attribute if custom logic is added and enforce test coverage.
    [Serializable]
    public class EntityAccessUnauthorizedException : Exception
    {
        public EntityAccessUnauthorizedException()
            : base()
        { }

        public EntityAccessUnauthorizedException(string message)
            : base(message)
        { }

        public EntityAccessUnauthorizedException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected EntityAccessUnauthorizedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
