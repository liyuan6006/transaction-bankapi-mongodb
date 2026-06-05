using BankApi.Interfaces;
using BankApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BankApi.Implementation
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<Customer> _customers;

        public CustomerRepository(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);

            var database = client.GetDatabase(settings.Value.DatabaseName);

            _customers = database.GetCollection<Customer>("Customers");
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var emailIndex =
                new CreateIndexModel<Customer>(
                    Builders<Customer>.IndexKeys
                        .Ascending(x => x.Email));

            _customers.Indexes.CreateOne(emailIndex);
        }

        public async Task<Customer?> GetByEmail(string email)
        {
            return await _customers
                .Find(x => x.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Customer>> GetAll()
        {
            return await _customers
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<Customer?> GetById(string id)
        {
            return await _customers
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task Create(Customer customer)
        {
            await _customers.InsertOneAsync(customer);
        }

        public async Task Update(Customer customer)
        {
            await _customers.ReplaceOneAsync(
                x => x.Id == customer.Id,
                customer);
        }

        public async Task Delete(string id)
        {
            await _customers.DeleteOneAsync(x => x.Id == id);
        }
    }
}