namespace LondonStockAPI.DTOs
{
    public class TradeReadDto
    {
        public int Id { get; set; }
        public string TickerSymbol { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
