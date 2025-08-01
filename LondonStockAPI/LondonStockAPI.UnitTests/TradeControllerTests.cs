using Moq;
using AutoMapper;
using LondonStockAPI.Controllers;
using LondonStockAPI.Data;
using LondonStockAPI.DTOs;
using LondonStockAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LondonStockAPI.UnitTests
{

    public class TradesControllerTests
    {
        private readonly Mock<ITradeRepo> _repoMock;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly TradesController _controller;

        public TradesControllerTests()
        {
            _repoMock = new Mock<ITradeRepo>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Stock, StockReadDto>();
                cfg.CreateMap<TradeCreatDto, Trade>();
            });
            _mapper = config.CreateMapper();
            _cache = new MemoryCache(new MemoryCacheOptions());

            _controller = new TradesController(_repoMock.Object, _mapper,_cache);
        }

        [Theory]
        [InlineData("RTO", 379.50, 10, 1)]
        [InlineData("RR", 1072, 10.5, 2)]
        public async Task SubmitTrade_ReturnsOk(string ticker,decimal price, decimal quantity,int brokerId)
        {
            //Arrange
            var dto = new TradeCreatDto { TickerSymbol = ticker, Price = price, Quantity = quantity, BrokerId = brokerId };
            _repoMock.Setup(r => r.CreateTradeAsync(It.IsAny<Trade>())).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.SubmitTrade(dto);

            //Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetStockValue_ReturnsOk_WhenStockExists()
        {
            //Arrange
            var stock = new Stock { TickerSymbol = "RTO", Price = 379.50m };
            _repoMock.Setup(r => r.GetStockAsync("RTO")).ReturnsAsync(stock);

            //Act
            var result = await _controller.GetStockValue("RTO");

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<StockReadDto>(okResult.Value);
            Assert.Equal("RTO", dto.TickerSymbol);
        }

        [Fact]
        public async Task GetStockValue_ReturnsNotFound_WhenStockDoesNotExist()
        {
            //Arrange
            _repoMock.Setup(r => r.GetStockAsync("MSFT")).ReturnsAsync(new Stock());

            //Act
            var result = await _controller.GetStockValue("MSFT");

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAllStockValues_ReturnsOkWithList()
        {
            //Arrange
            var stocks = new List<Stock>
            {
            new Stock { TickerSymbol = "RTO", Price = 379.50m },
            new Stock { TickerSymbol = "RR", Price = 1072m }
            };
            _repoMock.Setup(r => r.GetAllStocksAsync()).ReturnsAsync(stocks);

            //Act
            var result = await _controller.GetAllStockValues();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IList<StockReadDto>>(okResult.Value);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetSelectedStockValues_ReturnsOkWithFilteredList()
        {
            //Arrange
            var tickers = new List<string> { "RTO" };
            var stocks = new List<Stock> { new Stock { TickerSymbol = "RTO", Price = 379.50m } };
            _repoMock.Setup(r => r.GetStocksAsync(tickers)).ReturnsAsync(stocks);

            //Act
            var result = await _controller.GetSelectedStockValues(tickers);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IList<StockReadDto>>(okResult.Value);
            Assert.Single(list);
            Assert.Equal("RTO", list[0].TickerSymbol);
        }
    }

}
