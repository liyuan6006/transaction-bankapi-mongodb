using BankApi.Models;

namespace BankApi.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByEmail(string email);
        Task<List<Customer>> GetAll();

        Task<Customer?> GetById(string id);

        Task Create(Customer customer);

        Task Update(Customer customer);

        Task Delete(string id);
    }
}
