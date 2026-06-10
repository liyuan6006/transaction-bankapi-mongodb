namespace BankApi.DTO
{
    public class TransactionSearchRequest
    {
        public string? CustomerId { get; set; }

        public string? AccountId { get; set; }

        public string? Merchant { get; set; }

        public string? MerchantCategory { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Channel { get; set; }
        public string? Note { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public decimal? MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 25;
    }
}
