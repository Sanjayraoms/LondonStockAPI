namespace LondonStockAPI.DTOs
{
    public class StockReadDto
    {
        public string TickerSymbol { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
