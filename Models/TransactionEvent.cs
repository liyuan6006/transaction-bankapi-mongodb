using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BankApi.Models
{
    public class TransactionEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string TransactionId { get; set; }

        public string CustomerId { get; set; }

        public string AccountId { get; set; }

        public decimal Amount { get; set; }

        public string Merchant { get; set; }

        public string MerchantCategory { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public DateTime Timestamp { get; set; }

        public string Channel { get; set; }

        public bool IsInternational { get; set; }

        public string Note { get; set; }
    }
}
