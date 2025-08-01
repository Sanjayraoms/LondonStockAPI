using LondonStockAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LondonStockAPI.Data
{
    public class TradeRepo : ITradeRepo
    {
        private readonly AppDbContext _dbContext;

        public TradeRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateTradeAsync(Trade trade)
        {
            _dbContext.Add(trade);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Stock>> GetAllStocksAsync()
        {
            return await _dbContext.Trades.GroupBy(t => t.TickerSymbol)
                                          .Select(g => new Stock
                                          {
                                              TickerSymbol = g.Key,
                                              Price = g.Average(t => t.Price)
                                          }).ToListAsync();
        }

        public async Task<Stock> GetStockAsync(string tickerSymbol)
        {
            return await _dbContext.Trades.Where(t => t.TickerSymbol == tickerSymbol)
                                          .GroupBy(t => t.TickerSymbol)
                                          .Select(g => new Stock
                                          {
                                              TickerSymbol = tickerSymbol,
                                              Price = g.Average(m => m.Price)
                                          }).FirstOrDefaultAsync() ?? new Stock();
        }

        public async Task<List<Stock>> GetStocksAsync(List<string> tickerSymbols)
        {
            return await _dbContext.Trades.Where(t => tickerSymbols.Contains(t.TickerSymbol))
                                          .GroupBy(t => t.TickerSymbol)
                                          .Select(g => new Stock
                                          {
                                              TickerSymbol = g.Key,
                                              Price = g.Average(t => t.Price)
                                          }).ToListAsync();
        }
    }
}
