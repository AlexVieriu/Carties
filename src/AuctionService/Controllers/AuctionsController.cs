namespace AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _auctionDbContext;
    private readonly IMapper _mapping;

    public AuctionsController(AuctionDbContext auctionDbContext, IMapper mapping)
    {
        _auctionDbContext = auctionDbContext;
        _mapping = mapping;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        var auctions = await _auctionDbContext.Auctions
                        .Include(x => x.Item)
                        .OrderBy(x => x.Item.Make)
                        .ToListAsync();

        return _mapping.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _auctionDbContext.Auctions
                        .Include(x => x.Item)
                        .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
            return NotFound();

        return _mapping.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = _mapping.Map<Auction>(createAuctionDto);
        // TODO: add current user as seller

        auction.Seller = "test";

        _auctionDbContext.Auctions.Add(auction);
        var result = await _auctionDbContext.SaveChangesAsync() > 0;

        if (!result)
            return BadRequest("Could not save changes to the DB");

        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, _mapping.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _auctionDbContext.Auctions
                                .Include(x => x.Item)
                                .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null)
            return NotFound();

        // TODO: check seller == current user

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var result = await _auctionDbContext.SaveChangesAsync() > 0;

        if (!result)
            return BadRequest("Problem saving changes");

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _auctionDbContext.Auctions.FindAsync(id);

        if (auction == null)
            return NotFound();

        // TODO: check seller == current user

        _auctionDbContext.Auctions.Remove(auction);

        var result = await _auctionDbContext.SaveChangesAsync() > 0;

        if (!result)
            return BadRequest("Could not update DB");

        return Ok();
    }
}