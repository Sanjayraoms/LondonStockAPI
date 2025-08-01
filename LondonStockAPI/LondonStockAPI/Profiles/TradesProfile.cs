using AutoMapper;
using LondonStockAPI.DTOs;
using LondonStockAPI.Models;

namespace LondonStockAPI.Profiles
{
    public class TradesProfile : Profile
    {
        public TradesProfile()
        {
            //Source -> Target
            CreateMap<Stock, StockReadDto>();
            CreateMap<TradeCreatDto, Trade>();
        }
    }
}
