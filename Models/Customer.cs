using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BankApi.Models;

public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public decimal Balance { get; set; }

    public string Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public List<Address> Addresses { get; set; }
        = new();

    public List<Account> Accounts { get; set; }
        = new();
}