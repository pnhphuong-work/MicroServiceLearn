using System.Security.Cryptography;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
     public MappingProfiles()
     {
          
          CreateMap<Auction, AuctionDTO>().IncludeMembers(_ => _.Item);
          
          CreateMap<Item, AuctionDTO>();
          
          CreateMap<CreateAuctionDTO, Item>();
          
          //Mapping Item's properties from CreateAuctionDTO to Auction.Item
          CreateMap<CreateAuctionDTO, Auction>()
               .ForMember(d => d.Item, o => o.MapFrom(s => s));
     }
}
