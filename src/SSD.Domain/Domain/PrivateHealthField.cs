
namespace SSD.Domain
{
    public class PrivateHealthField : CustomField
    {
        public Provider Provider { get; set; }
        public int? ProviderId { get; set; }
    }
}
