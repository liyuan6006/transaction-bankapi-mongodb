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
    private readonly IMongoCollection<TransactionEvent>_transactions;
    public TransactionRepository(
       IMongoDatabase database)
    {
        _transactions =
            database.GetCollection<TransactionEvent>(
                "Transactions");
    }

    public async Task<List<TransactionEvent>> GetAll()
    {
        return await _transactions
            .Find(_ => true)
            .SortByDescending(x => x.Timestamp)
            .ToListAsync();
    }


    public async Task<List<TransactionEvent>>
       Search(
           TransactionSearchRequest request)
    {
        var filter =
            Builders<TransactionEvent>
                .Filter.Empty;

        if (!string.IsNullOrWhiteSpace(
            request.CustomerId))
        {
            filter &=
                Builders<TransactionEvent>
                    .Filter.Eq(
                        x => x.CustomerId,
                        request.CustomerId);
        }

        if (!string.IsNullOrWhiteSpace(
            request.AccountId))
        {
            filter &=
                Builders<TransactionEvent>
                    .Filter.Eq(
                        x => x.AccountId,
                        request.AccountId);
        }

        if (!string.IsNullOrWhiteSpace(
            request.Merchant))
        {
            filter &=
                Builders<TransactionEvent>
                    .Filter.Eq(
                        x => x.Merchant,
                        request.Merchant);
        }

        if (!string.IsNullOrWhiteSpace(
            request.MerchantCategory))
        {
            filter &=
                Builders<TransactionEvent>
                    .Filter.Eq(
                        x => x.MerchantCategory,
                        request.MerchantCategory);
        }

        if (!string.IsNullOrWhiteSpace(
            request.City))
        {
            filter &=
                Builders<TransactionEvent>
                    .Filter.Eq(
                        x => x.City,
                        request.City);
        }

        if (!string.IsNullOrWhiteSpace(
            request.State))
        {
            filter &=
                Builders<TransactionEvent>
                    .Filter.Eq(
                        x => x.State,
                        request.State);
        }

        if (!string.IsNullOrWhiteSpace(
            request.Channel))
        {
            filter &=
                Builders<TransactionEvent>
                    .Filter.Eq(
                        x => x.Channel,
                        request.Channel);
        }

        if (request.MinAmount.HasValue)
        {
            filter &=
                Builders<TransactionEvent>
                    .Filter.Gte(
                        x => x.Amount,
                        request.MinAmount.Value);
        }

        if (request.MaxAmount.HasValue)
        {
            filter &=
                Builders<TransactionEvent>
                    .Filter.Lte(
                        x => x.Amount,
                        request.MaxAmount.Value);
        }

        if (request.FromDate.HasValue)
        {
            filter &=
                Builders<TransactionEvent>
                    .Filter.Gte(
                        x => x.Timestamp,
                        request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            filter &=
                Builders<TransactionEvent>
                    .Filter.Lte(
                        x => x.Timestamp,
                        request.ToDate.Value);
        }
        if (!string.IsNullOrWhiteSpace(
        request.Note))
        {
            filter &=
                Builders<TransactionEvent>
                    .Filter.Text(
                        request.Note);
        }

        return await _transactions
            .Find(filter)
            .SortByDescending(
                x => x.Timestamp)
            .Skip(
                (request.PageNumber - 1)
                * request.PageSize)
            .Limit(
                request.PageSize)
            .ToListAsync();
    }



    public async Task<List<AutocompleteResult>>SearchAutocomplete(string term)
    {
        var pipeline = new[]
        {
        new BsonDocument(
            "$search",
            new BsonDocument
            {
                { "index", "autocomplete" },
                {
                    "compound",
                    new BsonDocument
                    {
                        {
                            "should",
                            new BsonArray
                            {
                                new BsonDocument(
                                    "autocomplete",
                                    new BsonDocument
                                    {
                                        { "query", term },
                                        { "path", "CustomerId" }
                                    }),

                                new BsonDocument(
                                    "autocomplete",
                                    new BsonDocument
                                    {
                                        { "query", term },
                                        { "path", "Merchant" }
                                    }),

                                new BsonDocument(
                                    "autocomplete",
                                    new BsonDocument
                                    {
                                        { "query", term },
                                        { "path", "MerchantCategory" }
                                    })


                            }
                        },
                        { "minimumShouldMatch", 1 }
                    }
                }
            }),

        new BsonDocument("$limit", 20)
    };

        var transactions =
            await _transactions
                .Aggregate<TransactionEvent>(
                    pipeline)
                .ToListAsync();

        var results =
            new List<AutocompleteResult>();

        foreach (var transaction in transactions)
        {
            if (!string.IsNullOrWhiteSpace(
                  transaction.CustomerId) &&
              transaction.CustomerId.Contains(
                  term,
                  StringComparison.OrdinalIgnoreCase))
            {
                results.Add(
                    new AutocompleteResult
                    {
                        Value =
                            transaction.CustomerId,
                        Field =
                            "CustomerId"
                    });
            }

            if (!string.IsNullOrWhiteSpace(
                    transaction.Merchant) &&
                transaction.Merchant.Contains(
                    term,
                    StringComparison.OrdinalIgnoreCase))
            {
                results.Add(
                    new AutocompleteResult
                    {
                        Value =
                            transaction.Merchant,
                        Field =
                            "Merchant"
                    });
            }

            if (!string.IsNullOrWhiteSpace(
                    transaction.MerchantCategory) &&
                transaction.MerchantCategory.Contains(
                    term,
                    StringComparison.OrdinalIgnoreCase))
            {
                results.Add(
                    new AutocompleteResult
                    {
                        Value =
                            transaction.MerchantCategory,
                        Field =
                            "MerchantCategory"
                    });
            }
        }

        return results
            .DistinctBy(x => x.Value)
            .Take(20)
            .ToList();
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



    public async Task<List<TransactionEvent>>GetTransactions_traditional_pagination(
        int pageNumber,
        int pageSize)
    {
        return await _transactions
            .Find(_ => true)
            .SortByDescending(x => x.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
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

    public async Task Create(
    TransactionEvent transaction)
    {
        await _transactions
            .InsertOneAsync(
                transaction);
    }

    public async Task<TransactionEvent?>
    GetById(string id)
    {
        return await _transactions
            .Find(x =>
                x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task Update(
    TransactionEvent transaction)
    {
        await _transactions
            .ReplaceOneAsync(
                x => x.Id == transaction.Id,
                transaction);
    }

    public async Task Delete(
    string id)
    {
        await _transactions
            .DeleteOneAsync(
                x => x.Id == id);
    }

}