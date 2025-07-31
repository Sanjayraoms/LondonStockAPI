using System.ComponentModel.DataAnnotations;

namespace LondonStockAPI.DTOs
{
    public class TradeCreatDto
    {
        [Required]
        public string TickerSymbol { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public int BrokerId { get; set; }
    }
}
