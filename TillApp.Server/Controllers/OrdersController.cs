using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TillApp.Shared.Models;

namespace TillApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            _logger.LogInformation("Fetching all orders.");
            return await _context.Orders.Include(o => o.OrderItems).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                                               .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                _logger.LogWarning($"Order with ID {id} not found.");
                return NotFound();
            }

            return order;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Order with ID {order.OrderID} added.");
            return CreatedAtAction("GetOrder", new { id = order.OrderID }, order);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.OrderID)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Order with ID {id} updated.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    _logger.LogWarning($"Order with ID {id} not found for update.");
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
        public async Task<ActionResult<Order>> DeleteOrder(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                                              .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                _logger.LogWarning($"Order with ID {id} not found for deletion.");
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Order with ID {id} deleted.");

            return order;
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}
