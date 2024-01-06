namespace AuctionService.Entities;

[Table("Items")]
public class Item
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public int Mileage { get; set; }
    public string ImageURL { get; set; }

    // nav properties
    public Auction Auction { get; set; }
    public Guid AuctionId { get; set; }     // EF will know that this is a FK
}