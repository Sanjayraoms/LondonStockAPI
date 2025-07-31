using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LondonStockAPI.Models
{
    public class Trade
    {
        public int Id { get; set; }
        [Required]
        public string TickerSymbol { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public int BrokerId { get; set; }
        [Required]
        public DateTime TradeTime { get; set; } = DateTime.UtcNow;
    }
}
