using BankApi.Models;

namespace BankApi.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<TransactionEvent>> GetAll();

        Task<TransactionEvent?> GetByTransactionId(
            string transactionId);

        Task<List<TransactionEvent>> GetByCustomer(
            string customerId);

        Task<List<TransactionEvent>> SearchNotes(string searchTerm);
    }
}
