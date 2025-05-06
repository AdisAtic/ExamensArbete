using Microsoft.AspNetCore.Mvc;
using PriceTrackingAPI.Models;
using PriceTrackingAPI.Services;
using System.Threading;

namespace PriceTrackingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductPricesController : ControllerBase
    {
        private readonly IProductPriceService _service;
        private readonly IPriceUpdateService _updater;

        public ProductPricesController(
            IProductPriceService service,
            IPriceUpdateService updater)
        {
            _service = service;
            _updater = updater;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductPrice>>> GetProductPrices()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductPrice>> GetProductPrice(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet("lowest/{productId}")]
        public async Task<ActionResult> GetLowestPrice(string productId)
        {
            var lowest = await _service.GetLowestPriceLast30DaysAsync(productId);
            if (lowest <= 0)
                return NotFound($"No prices in last 30 days for {productId}");
            return Ok(new { productId, lowestPrice = lowest });
        }

        [HttpPost]
        public async Task<ActionResult<ProductPrice>> PostProductPrice(ProductPrice productPrice)
        {
            var created = await _service.CreateAsync(productPrice);
            return CreatedAtAction(nameof(GetProductPrice), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductPrice(int id, ProductPrice productPrice)
        {
            if (id != productPrice.Id)
                return BadRequest();
            await _service.UpdateAsync(productPrice);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductPrice(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshPrices()
        {
            await _updater.UpdatePricesAsync(CancellationToken.None);
            return NoContent();
        }

        [HttpGet("latest/{productId}")]
        public async Task<ActionResult> GetLatestPrice(string productId)
        {
            var latest = await _service.GetLatestPriceAsync(productId);
            if (latest == null)
                return NotFound($"No price found for {productId}");
            return Ok(new { productId = latest.ProductId, price = latest.Price, recordedAt = latest.RecordedAt });
        }
    }
}