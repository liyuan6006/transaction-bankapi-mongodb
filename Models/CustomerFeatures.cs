namespace BankApi.Models
{
    public class CustomerFeatures
    {
        public decimal Avg7Day { get; set; }

        public int TransactionCount24h { get; set; }

        public bool DeviceChanged { get; set; }
    }
}
