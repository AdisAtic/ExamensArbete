using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PriceTrackingAPI.Services.FeedParser
{
    public interface IFeedParser
    {
        Task<Dictionary<string, (decimal price, decimal? salePrice)>> ParseAsync(CancellationToken ct);
    }
}
