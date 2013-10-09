using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SSD
{
    [ExcludeFromCodeCoverage] // NOTE: This class does not implement custom logic.  Remove this attribute if custom logic is added and enforce test coverage.
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
            : base()
        { }

        public EntityNotFoundException(string message)
            : base(message)
        { }

        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
