using BankApi.Interfaces;
using BankApi.Models;
using Microsoft.Extensions.Options;
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

    public async Task<List<TransactionEvent>> SearchNotes(string searchTerm)
    {
        var filter =
            Builders<TransactionEvent>
                .Filter
                .Text(searchTerm);

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

    public async Task<List<TransactionEvent>>
        GetByCustomer(
            string customerId)
    {
        return await _transactions
            .Find(x =>
                x.CustomerId ==
                customerId)
            .SortByDescending(x =>
                x.Timestamp)
            .ToListAsync();
    }
}