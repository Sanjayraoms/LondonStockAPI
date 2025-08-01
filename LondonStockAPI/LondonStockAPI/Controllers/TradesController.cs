using AutoMapper;
using LondonStockAPI.Data;
using LondonStockAPI.DTOs;
using LondonStockAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace LondonStockAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradesController : ControllerBase
    {
        private readonly ITradeRepo _repository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;


        public TradesController(ITradeRepo repository, IMapper mapper, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitTrade(TradeCreatDto tradeCreateDto)
        {
            var trade = _mapper.Map<Trade>(tradeCreateDto);
            await _repository.CreateTradeAsync(trade);
            _cache.Remove(tradeCreateDto.TickerSymbol); // Remove from cache to ensure fresh data
            return Ok();
        }

        [HttpGet("stock/{ticker}")]
        public async Task<ActionResult<StockReadDto>> GetStockValue(string ticker)
        {
            if (!_cache.TryGetValue(ticker,out Stock? stockCached))
            {
                var stock = await _repository.GetStockAsync(ticker);
                if (!string.IsNullOrEmpty(stock.TickerSymbol))
                {
                    _cache.Set(ticker, stock); // Add to cache
                    return Ok(_mapper.Map<StockReadDto>(stock));
                }
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<StockReadDto>(stockCached));
            }
        }

        [HttpGet("stocks")]
        public async Task<ActionResult<List<StockReadDto>>> GetAllStockValues()
        {
            //We can add all the stocks into the _cache. So that it will be helpful when
            //getting selected stocks
            var result = await _repository.GetAllStocksAsync();
            Parallel.ForEach(result, stock =>
            {
                _cache.Set(stock.TickerSymbol, stock); // Add to cache
            });
            return Ok(_mapper.Map<IList<StockReadDto>>(result));
        }

        [HttpPost("stocks/filter")]
        public async Task<ActionResult<List<StockReadDto>>> GetSelectedStockValues([FromBody] List<string> tickers)
        {
            var result = new ConcurrentBag<Stock>();
            var missingTickers = new List<string>();

            foreach (var ticker in tickers)
            {
                if (_cache.TryGetValue(ticker, out Stock? cachedStock))
                {
                    result.Add(cachedStock);
                }
                else
                {
                    missingTickers.Add(ticker);
                }
            }
            if (missingTickers.Count > 0)
            {
                var stocksFromRepo = await _repository.GetStocksAsync(missingTickers);
                Parallel.ForEach(stocksFromRepo, stock =>
                {
                    _cache.Set(stock.TickerSymbol, stock); // Add to cache
                    result.Add(stock); // ConcurrentBag is thread-safe, no need for locking
                });
            }
            return Ok(_mapper.Map<IList<StockReadDto>>(result));
        }
    }

}
