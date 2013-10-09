using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SSD.IO
{
    [ExcludeFromCodeCoverage] // NOTE: This class does not implement custom logic.  Remove this attribute if custom logic is added and enforce test coverage.
    [Serializable]
    public class BlobException : Exception
    {
        public BlobException()
            : base()
        { }

        public BlobException(string message)
            : base(message)
        { }

        public BlobException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected BlobException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
