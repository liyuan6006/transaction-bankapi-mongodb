using BankApi.DTO;
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

        Task<List<TransactionSearchResult>> SearchNotes(string searchTerm);
        Task<List<TransactionEvent>>SearchNotesFuzzy(string searchTerm);
        Task<List<TransactionEvent>>SearchNotesRegex(string searchTerm);
    }
}
