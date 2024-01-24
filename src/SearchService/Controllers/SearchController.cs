using SearchService.RequestHelpers;

namespace SearchService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
    {
        // creating the query
        var query = DB.PagedSearch<Item, Item>();

        // we find out if we have a search term, if not, we are going to match it
        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();

        query = searchParams.OrderBy switch
        {
            "make" => query.Sort(x => x.Ascending(a => a.Make)),
            "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
            _ => query.Sort(x => x.Ascending(a => a.AuctionEnd)) // auctions ending soonest
        };

        // filter here is just a field with multiple values
        query = searchParams.FilterBy switch
        {
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) &&
                                             x.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };

        // here is doing filtering bt Seller
        if (!string.IsNullOrEmpty(searchParams.Seller))
            query.Match(x => x.Seller == searchParams.Seller);

        // here is doing filtering bt Winner
        if (!string.IsNullOrEmpty(searchParams.Winner))
            query.Match(x => x.Winner == searchParams.Winner);

        query.PageNumber(searchParams.PageNumber);
        query.PageSize(searchParams.PageSize);

        var result = await query.ExecuteAsync();

        return Ok(new
        {
            results = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}