using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController : ControllerBase
{
     private readonly AuctionDbContext _context;
     private readonly IMapper _mapper;

     public AuctionController(AuctionDbContext context, IMapper mapper)
     {
          _context = context;
          _mapper = mapper;
     }
     [HttpGet]
     public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions()
     {
          var auctions = await _context.Auctions
               .Include(x => x.Item)
               .OrderBy(x => x.Item.Make)
               .ToListAsync();
          return _mapper.Map<List<AuctionDTO>>(auctions);
     }
     [HttpGet("{id}")]
     public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
     {
          var auction = await _context.Auctions
               .Include(x => x.Item)
               .FirstOrDefaultAsync(x => x.Id == id);
          if (auction is null)
          {
               return NotFound();
          } 
          return _mapper.Map<AuctionDTO>(auction);
     }
     [HttpPost]
     public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDTO)
     {
          var auction = _mapper.Map<Auction>(auctionDTO);
          //TODO: Add current user as seller
          auction.Seller = "test";
          _context.Auctions.Add(auction);

          var result = await _context.SaveChangesAsync() > 0;

          if (!result)
          {
               return BadRequest("Couldn't save changes in DB");
          }
          return CreatedAtAction(nameof(GetAuctionById), 
               new {auction.Id}, _mapper.Map<AuctionDTO>(auction));
     }
     [HttpPut("{id}")]
     public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDTO updateAuctionDTO)
     {
          var auction = await _context.Auctions
               .Include(x => x.Item)
               .FirstOrDefaultAsync(x => x.Id == id);
          if (auction is null)
          {
               return NotFound();
          }
          //TODO: check seller == username
          auction.Item.Make = updateAuctionDTO.Make ?? auction.Item.Make;
          auction.Item.Model = updateAuctionDTO.Model ?? auction.Item.Model;
          auction.Item.Year = updateAuctionDTO.Year ?? auction.Item.Year;
          auction.Item.Color = updateAuctionDTO.Color ?? auction.Item.Color;
          auction.Item.Mileage = updateAuctionDTO.Mileage ?? auction.Item.Mileage;

          var result = _context.SaveChanges() > 0;
          if (!result)
          {
               return BadRequest("Couldn't save changes in DB");
          }
          return Ok();
     }
     [HttpDelete("{id}")]
     public async Task<ActionResult> DeleteAuction(Guid id)
     {
          var auction = await _context.Auctions.FindAsync(id);
          if (auction is null) return NotFound();
          //TODO: check seller == username
          _context.Auctions.Remove(auction);
          var result = _context.SaveChanges() > 0;
          if (!result)
          {
               return BadRequest("Couldn't save changes in DB");
          }
          return Ok();
     }

}
