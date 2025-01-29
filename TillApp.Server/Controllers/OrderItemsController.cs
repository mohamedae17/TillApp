using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TillApp.Server.Models;

namespace TillApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderItemsController> _logger;

        public OrderItemsController(ApplicationDbContext context, ILogger<OrderItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
        {
            _logger.LogInformation("Fetching all order items.");
            return await _context.OrderItems.Include(oi => oi.Order).ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.Include(oi => oi.Order)
                                                      .FirstOrDefaultAsync(oi => oi.OrderItemID == id);

            if (orderItem == null)
            {
                _logger.LogWarning($"OrderItem with ID {id} not found.");
                return NotFound();
            }

            return orderItem;
        }


        [HttpPost]
        public async Task<ActionResult<OrderItem>> PostOrderItem(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"OrderItem with ID {orderItem.OrderItemID} added.");

            return CreatedAtAction("GetOrderItem", new { id = orderItem.OrderItemID }, orderItem);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderItem(int id, OrderItem orderItem)
        {
            if (id != orderItem.OrderItemID)
            {
                return BadRequest();
            }

            _context.Entry(orderItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"OrderItem with ID {id} updated.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderItemExists(id))
                {
                    _logger.LogWarning($"OrderItem with ID {id} not found for update.");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<OrderItem>> DeleteOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.Include(oi => oi.Order)
                                                      .FirstOrDefaultAsync(oi => oi.OrderItemID == id);

            if (orderItem == null)
            {
                _logger.LogWarning($"OrderItem with ID {id} not found for deletion.");
                return NotFound();
            }

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"OrderItem with ID {id} deleted.");

            return orderItem;
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.OrderItemID == id);
        }
    }
}