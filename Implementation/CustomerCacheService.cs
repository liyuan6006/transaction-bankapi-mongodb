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

        public async Task RemoveCustomerCache(string customerId)
        {
            await _redis.KeyDeleteAsync(
                $"customer:{customerId}");
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


        public async Task SaveFeatures(string customerId,CustomerFeatures features)
        {
            await _redis.HashSetAsync(
                $"customer:{customerId}:features",
                new HashEntry[]
                {
                    new("avg_7d",
                        features.Avg7Day.ToString()),

                    new("txn_count_24h",
                        features.TransactionCount24h),

                    new("device_changed",
                        features.DeviceChanged ? 1 : 0)
                });
        }

        public async Task<CustomerFeatures?> GetFeatures(string customerId)
        {
            var values =
                await _redis.HashGetAllAsync(
                    $"customer:{customerId}:features");

            if (values.Length == 0)
                return null;

            return new CustomerFeatures
            {
                Avg7Day =
                    decimal.Parse(
                        values.First(
                            x => x.Name ==
                                 "avg_7d")
                        .Value),

                TransactionCount24h =
                    int.Parse(
                        values.First(
                            x => x.Name ==
                                 "txn_count_24h")
                        .Value),

                DeviceChanged =
                    values.First(
                        x => x.Name ==
                             "device_changed")
                        .Value == "1"
            };
        }
    }
}
