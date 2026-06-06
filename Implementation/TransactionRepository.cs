using BankApi.DTO;
using BankApi.Interfaces;
using BankApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BankApi.Implementation;

public class TransactionRepository
    : ITransactionRepository
{
    private readonly IMongoCollection<TransactionEvent>
        _transactions;

    public TransactionRepository(
        IOptions<MongoSettings> settings)
    {
        var client =
            new MongoClient(
                settings.Value.ConnectionString);

        var database =
            client.GetDatabase(
                settings.Value.DatabaseName);

        _transactions =
            database.GetCollection<TransactionEvent>(
                "Transactions");
    }

    public async Task<List<TransactionSearchResult>> SearchNotes(string searchTerm)
    {
        var filter =
            Builders<TransactionEvent>
                .Filter
                .Text(searchTerm);

        var projection =
            Builders<TransactionEvent>
                .Projection
                .Include(x => x.TransactionId)
                .Include(x => x.Note)
                .MetaTextScore("score");

        var results =
            await _transactions
                .Find(filter)
                .Project<BsonDocument>(projection)
                .Sort(new BsonDocument(
                    "score",
                    new BsonDocument("$meta", "textScore")))
                .ToListAsync();

        return results.Select(x =>
            new TransactionSearchResult
            {
                TransactionId =
                    x["TransactionId"].AsString,

                Note =
                    x["Note"].AsString,

                Score =
                    x["score"].AsDouble
            }).ToList();
    }

    public async Task<List<TransactionEvent>> SearchNotesFuzzy(string searchTerm)
    {
        var pipeline = new[]
        {
        new BsonDocument("$search",
            new BsonDocument
            {
                {
                    "index",
                    "default"
                },
                {
                    "text",
                    new BsonDocument
                    {
                        { "query", searchTerm },
                        { "path", "Note" },
                        {
                            "fuzzy",
                            new BsonDocument
                            {
                                { "maxEdits", 2 }
                            }
                        }
                    }
                }
            })
    };

        return await _transactions
            .Aggregate<TransactionEvent>(pipeline)
            .ToListAsync();
    }

    public async Task<List<TransactionEvent>>
    SearchNotesRegex(string searchTerm)
    {
        var filter =
            Builders<TransactionEvent>
                .Filter
                .Regex(
                    x => x.Note,
                    new BsonRegularExpression(
                        searchTerm,
                        "i"));

        return await _transactions
            .Find(filter)
            .SortByDescending(x => x.Timestamp)
            .ToListAsync();
    }
    public async Task<List<TransactionEvent>> GetAll()
    {
        return await _transactions
            .Find(_ => true)
            .SortByDescending(x => x.Timestamp)
            .ToListAsync();
    }

    public async Task<TransactionEvent?>
        GetByTransactionId(
            string transactionId)
    {
        return await _transactions
            .Find(x =>
                x.TransactionId ==
                transactionId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<TransactionEvent>> GetByCustomer(string customerId)
    {
        return await _transactions
            .Find(x =>
                x.CustomerId ==
                customerId)
            .SortByDescending(x =>
                x.Timestamp)
            .ToListAsync();
    }


    public async Task<List<CustomerSpending>> GetTopCustomers()
    {
        var pipeline = new[]
        {
        new BsonDocument("$group",
            new BsonDocument
            {
                { "_id", "$CustomerId" },
                {
                    "TotalSpent",
                    new BsonDocument("$sum", "$Amount")
                }
            }),

        new BsonDocument("$sort",
            new BsonDocument("TotalSpent", -1)),

        new BsonDocument("$limit", 2)
    };

        var results = await _transactions
            .Aggregate<BsonDocument>(pipeline)
            .ToListAsync();

        return results
            .Select(x => new CustomerSpending
            {
                CustomerId =
                    x["_id"].AsString,

                TotalSpent =
                    (decimal)x["TotalSpent"].ToDouble()
            })
            .ToList();
    }

}