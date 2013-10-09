using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SSD.Business
{
    [ExcludeFromCodeCoverage] // NOTE: This class does not implement custom logic.  Remove this attribute if custom logic is added and enforce test coverage.
    [Serializable]
    public class LicenseAgreementException : Exception
    {
        public LicenseAgreementException()
            : base()
        { }

        public LicenseAgreementException(string message)
            : base(message)
        { }

        public LicenseAgreementException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected LicenseAgreementException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
