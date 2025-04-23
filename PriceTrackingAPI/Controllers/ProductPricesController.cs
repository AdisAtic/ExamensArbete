using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceTrackingAPI.Data;
using PriceTrackingAPI.Models;
using PriceTrackingAPI.Services;
using System.Threading;

namespace PriceTrackingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductPricesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPriceUpdateService _updater;

        public ProductPricesController(
            AppDbContext context,
            IPriceUpdateService updater)
        {
            _context = context;
            _updater = updater;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductPrice>>> GetProductPrices()
        {
            return await _context.ProductPrices.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductPrice>> GetProductPrice(int id)
        {
            var productPrice = await _context.ProductPrices.FindAsync(id);

            if (productPrice == null)
            {
                return NotFound();
            }

            return productPrice;
        }

        /// <summary>
        /// Hämtar det lägsta priset för en produkt de senaste 30 dagarna.
        /// </summary>
        [HttpGet("lowest/{productId}")]
        public async Task<ActionResult> GetLowestPrice(string productId)
        {
            var fromDate = DateTime.UtcNow.AddDays(-30);

            var lowestPrice = await _context.ProductPrices
                .Where(p => p.ProductId == productId && p.RecordedAt >= fromDate)
                .OrderBy(p => p.LowestPriceLast30Days)
                .Select(p => p.LowestPriceLast30Days)
                .FirstOrDefaultAsync();
            
            if (lowestPrice == 0)
            {
                return NotFound($"Inget pris registrerat de senaste 30 dagarna för produkt {productId}");
            }

            return Ok(new { productId, lowestPrice });
        }

        /// <summary>
        /// Skapar ett nytt pris, främst för intern användning.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductPrice>> PostProductPrice(ProductPrice productPrice)
        {
            _context.ProductPrices.Add(productPrice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductPrice), new { id = productPrice.Id }, productPrice);
        }

        /// <summary>
        /// Uppdaterar ett existerande pris via dess ID.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductPrice(int id, ProductPrice productPrice)
        {
            if (id != productPrice.Id)
            {
                return BadRequest();
            }

            _context.Entry(productPrice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ProductPrices.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Raderar ett pris via dess ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductPrice(int id)
        {
            var productPrice = await _context.ProductPrices.FindAsync(id);
            if (productPrice == null)
            {
                return NotFound();
            }

            _context.ProductPrices.Remove(productPrice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Triggerar en manuell prisuppdatering från Litium/GoogleShopping.
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshPrices()
        {
            await _updater.UpdatePricesAsync(CancellationToken.None);
            return NoContent();
        }
    }
}
