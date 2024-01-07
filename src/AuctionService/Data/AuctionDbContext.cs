using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions opt) : base(opt)
    {

    }
    public DbSet<Auction> Auctions { get; set; }
}
