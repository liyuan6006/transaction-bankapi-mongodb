using BankApi.Interfaces;
using BankApi.Models;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace BankApi.Implementation
{
    public class CustomerCacheService: ICustomerCacheService
    {
        private readonly IDatabase _redis;
        private readonly IMongoCollection<Customer> _customers;

        public CustomerCacheService(
            IConnectionMultiplexer connection,
            IOptions<MongoSettings> settings)
        {
            _redis = connection.GetDatabase();

            var client =
                new MongoClient(
                    settings.Value.ConnectionString);

            var database =
                client.GetDatabase(
                    settings.Value.DatabaseName);

            _customers =
                database.GetCollection<Customer>(
                    "Customers");
        }


        public async Task<Customer?> GetCustomer(string customerId)
        {
            var cacheKey =
                $"customer:{customerId}";

            var cached =
                await _redis.StringGetAsync(
                    cacheKey);

            if (cached.HasValue)
            {
                Console.WriteLine(
                    "Returned from Redis");

                return JsonSerializer
                    .Deserialize<Customer>(
                        cached!);
            }

            Console.WriteLine(
                "Returned from MongoDB");

            var customer =
                await _customers
                    .Find(x =>
                        x.Id ==
                        customerId)
                    .FirstOrDefaultAsync();

            if (customer != null)
            {
                await _redis.StringSetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(
                        customer),
                    TimeSpan.FromMinutes(30));
            }

            return customer;
        }
    }
}
