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
}