using BankApi.Models;

namespace BankApi.Interfaces
{
    public interface ICustomerCacheService
    {
        Task<Customer?> GetCustomer(
           string customerId);
        Task RemoveCustomerCache(
    string customerId);
    }
}
