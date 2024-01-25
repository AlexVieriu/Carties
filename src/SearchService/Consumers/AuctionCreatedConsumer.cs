﻿namespace SearchService.Consumers;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly IMapper _mapper;

    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        WriteLine("--> Consuming auction created: " + context.Message);

        var item = _mapper.Map<Item>(context.Message);

        await item.SaveAsync();
    }
}
