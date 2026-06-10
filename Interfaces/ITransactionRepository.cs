using BankApi.DTO;
using BankApi.Models;

namespace BankApi.Interfaces
{
    public interface ITransactionRepository
    {

        Task<List<TransactionEvent>> GetAll();
        Task<List<TransactionEvent>> Search(TransactionSearchRequest request);
        Task<List<AutocompleteResult>>SearchAutocomplete(string term);







        Task<List<TransactionEvent>> GetTransactions_traditional_pagination(int pageNumber,int pageSize);
        Task<TransactionEvent?> GetById(string id);
        Task Create(TransactionEvent transaction);

        Task Update(TransactionEvent transaction);

        Task Delete( string id);
        Task<TransactionEvent?> GetByTransactionId(string transactionId);

        Task<List<TransactionEvent>> GetByCustomer(
            string customerId);

        Task<List<TransactionSearchResult>> SearchNotes(string searchTerm);
        Task<List<TransactionEvent>>SearchNotesFuzzy(string searchTerm);
        Task<List<TransactionEvent>>SearchNotesRegex(string searchTerm);
        Task<List<CustomerSpending>>GetTopCustomers();
    }
}
