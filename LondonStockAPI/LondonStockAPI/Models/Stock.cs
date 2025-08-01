namespace LondonStockAPI.Models
{
    public class Stock
    {
        public string TickerSymbol { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
