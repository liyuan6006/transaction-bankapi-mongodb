using BankApi.Models;

namespace BankApi.Interfaces
{
    public interface ICustomerCacheService
    {
        Task<Customer?> GetCustomer(string customerId);
        Task RemoveCustomerCache(string customerId);
        Task SaveFeatures(string customerId, CustomerFeatures features);
        Task<CustomerFeatures?> GetFeatures(string customerId);
    }
}
