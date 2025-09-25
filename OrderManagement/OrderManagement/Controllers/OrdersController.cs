using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Data;
using OrderManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace OrderManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public OrdersController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IEnumerable<OrderDto>> Get() =>
            await _db.Orders
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto(o.Id, o.Title, o.Amount, o.Status, o.CreatedAt))
                .ToListAsync();

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto)
        {
            var userId = await _db.Users
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            var o = new Order { Title = dto.Title, Amount = dto.Amount, UserId = userId };
            _db.Orders.Add(o);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = o.Id },
                new OrderDto(o.Id, o.Title, o.Amount, o.Status, o.CreatedAt));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateOrderDto dto)
        {
            var o = await _db.Orders.FindAsync(id);
            if (o is null) return NotFound();
            o.Title = dto.Title; o.Amount = dto.Amount; o.Status = dto.Status;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var o = await _db.Orders.FindAsync(id);
            if (o is null) return NotFound();
            _db.Remove(o); await _db.SaveChangesAsync();
            return NoContent();
        }

        public record OrderDto(Guid Id, string Title, decimal Amount, string Status, DateTime CreatedAt);
        public record CreateOrderDto(string Title, decimal Amount);
        public record UpdateOrderDto(string Title, decimal Amount, string Status);
    }
}
