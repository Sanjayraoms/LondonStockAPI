using LondonStockAPI.Models;

namespace LondonStockAPI.Data
{
    public interface ITradeRepo
    {
        Task<List<Stock>> GetAllStocksAsync();
        Task<List<Stock>> GetStocksAsync(List<string> tickerSymbols);
        Task<Stock> GetStockAsync(string tickerSymbol);
        Task CreateTradeAsync(Trade trade);
    }
}
