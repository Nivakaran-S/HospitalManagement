using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryService.Data;
using InventoryService.Models;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryContext _context;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(InventoryContext context, ILogger<InventoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventoryItems(
            [FromQuery] string? category,
            [FromQuery] string? search)
        {
            var query = _context.InventoryItems
                .Include(ii => ii.Transactions)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(ii => ii.Category == category);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(ii => ii.ItemName.Contains(search) || 
                                         ii.ItemCode.Contains(search));

            var items = await query
                .Where(ii => ii.IsActive)
                .OrderBy(ii => ii.ItemName)
                .ToListAsync();

            return Ok(items);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryItem>> GetInventoryItem(int id)
        {
            var item = await _context.InventoryItems
                .Include(ii => ii.Transactions)
                .FirstOrDefaultAsync(ii => ii.Id == id);

            if (item == null) return NotFound();
            return item;
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost]
        public async Task<ActionResult<InventoryItem>> PostInventoryItem(InventoryItem item)
        {
            if (await _context.InventoryItems.AnyAsync(ii => ii.ItemCode == item.ItemCode))
                return BadRequest("Item code already exists");

            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;

            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInventoryItem), new { id = item.Id }, item);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventoryItem(int id, InventoryItem item)
        {
            if (id != item.Id) return BadRequest();

            var dbItem = await _context.InventoryItems.FindAsync(id);
            if (dbItem == null) return NotFound();

            dbItem.ItemName = item.ItemName;
            dbItem.Category = item.Category;
            dbItem.Unit = item.Unit;
            dbItem.UnitPrice = item.UnitPrice;
            dbItem.Location = item.Location;
            dbItem.Description = item.Description;
            dbItem.ReorderLevel = item.ReorderLevel;
            dbItem.Supplier = item.Supplier;
            dbItem.IsActive = item.IsActive;
            dbItem.UpdatedAt = DateTime.UtcNow;

            _context.Entry(dbItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.InventoryItems.AnyAsync(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null) return NotFound();

            item.IsActive = false;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("{id}/transaction")]
        public async Task<ActionResult<InventoryTransaction>> RecordTransaction(int id, InventoryTransaction transaction)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null) return NotFound("Inventory item not found");

            transaction.InventoryItemId = id;
            transaction.TransactionDate = DateTime.UtcNow;

            _context.InventoryTransactions.Add(transaction);

            // Update quantity based on transaction type
            switch (transaction.TransactionType)
            {
                case "Purchase":
                    item.Quantity += transaction.Quantity;
                    break;
                case "Issue":
                    if (item.Quantity < transaction.Quantity)
                        return BadRequest("Insufficient quantity");
                    item.Quantity -= transaction.Quantity;
                    break;
                case "Return":
                    item.Quantity += transaction.Quantity;
                    break;
                case "Adjustment":
                    item.Quantity = transaction.Quantity;
                    break;
            }

            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInventoryItem), new { id = item.Id }, transaction);
        }

        [Authorize(Roles = "staff,doctor")]
        [HttpPost("request")]
        public async Task<ActionResult<InventoryRequest>> CreateRequest(InventoryRequest request)
        {
            var item = await _context.InventoryItems.FindAsync(request.InventoryItemId);
            if (item == null) return NotFound("Inventory item not found");

            request.ItemName = item.ItemName;
            request.RequestDate = DateTime.UtcNow;
            request.Status = "Pending";

            _context.InventoryRequests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRequest), new { id = request.Id }, request);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("requests")]
        public async Task<ActionResult<IEnumerable<InventoryRequest>>> GetRequests([FromQuery] string? status)
        {
            var query = _context.InventoryRequests
                .Include(ir => ir.InventoryItem)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(ir => ir.Status == status);

            var requests = await query
                .OrderByDescending(ir => ir.RequestDate)
                .ToListAsync();

            return Ok(requests);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("request/{id}")]
        public async Task<ActionResult<InventoryRequest>> GetRequest(int id)
        {
            var request = await _context.InventoryRequests
                .Include(ir => ir.InventoryItem)
                .FirstOrDefaultAsync(ir => ir.Id == id);

            if (request == null) return NotFound();
            return request;
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("request/{id}/approve")]
        public async Task<IActionResult> ApproveRequest(int id, [FromBody] string approvedBy)
        {
            var request = await _context.InventoryRequests
                .Include(ir => ir.InventoryItem)
                .FirstOrDefaultAsync(ir => ir.Id == id);

            if (request == null) return NotFound();

            if (request.Status != "Pending")
                return BadRequest("Request is not pending");

            var item = request.InventoryItem;
            if (item.Quantity < request.RequestedQuantity)
                return BadRequest("Insufficient inventory");

            request.Status = "Approved";
            request.ApprovedBy = approvedBy;
            request.ApprovedDate = DateTime.UtcNow;

            // Create issue transaction
            var transaction = new InventoryTransaction
            {
                InventoryItemId = request.InventoryItemId,
                TransactionType = "Issue",
                Quantity = request.RequestedQuantity,
                UnitPrice = item.UnitPrice,
                RequestedBy = request.RequestedBy,
                ApprovedBy = approvedBy,
                Department = request.Department,
                Purpose = request.Purpose,
                TransactionDate = DateTime.UtcNow
            };
            _context.InventoryTransactions.Add(transaction);

            // Update quantity
            item.Quantity -= request.RequestedQuantity;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("request/{id}/reject")]
        public async Task<IActionResult> RejectRequest(int id, [FromBody] string reason)
        {
            var request = await _context.InventoryRequests.FindAsync(id);
            if (request == null) return NotFound();

            if (request.Status != "Pending")
                return BadRequest("Request is not pending");

            request.Status = "Rejected";
            request.RejectionReason = reason;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetLowStockItems()
        {
            var items = await _context.InventoryItems
                .Where(ii => ii.IsActive && ii.Quantity <= ii.ReorderLevel)
                .OrderBy(ii => ii.Quantity)
                .ToListAsync();

            return Ok(items);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("report/value")]
        public async Task<ActionResult<object>> GetInventoryValue()
        {
            var items = await _context.InventoryItems
                .Where(ii => ii.IsActive)
                .ToListAsync();

            var report = new
            {
                TotalValue = items.Sum(ii => ii.Quantity * ii.UnitPrice),
                TotalItems = items.Sum(ii => ii.Quantity),
                NumberOfProducts = items.Count,
                LowStockCount = items.Count(ii => ii.Quantity <= ii.ReorderLevel),
                OutOfStockCount = items.Count(ii => ii.Quantity == 0)
            };

            return Ok(report);
        }
    }
}