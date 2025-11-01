using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyService.Data;
using PharmacyService.Models;

namespace PharmacyService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PharmacyController : ControllerBase
    {
        private readonly PharmacyContext _context;
        private readonly ILogger<PharmacyController> _logger;

        public PharmacyController(PharmacyContext context, ILogger<PharmacyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medicine>>> GetMedicines(
            [FromQuery] string? search,
            [FromQuery] string? category,
            [FromQuery] bool? inStock)
        {
            var query = _context.Medicines
                .Include(m => m.StockHistory)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(m => m.Name.Contains(search) || 
                                        m.GenericName.Contains(search));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(m => m.Category == category);

            if (inStock.HasValue && inStock.Value)
                query = query.Where(m => m.QuantityInStock > 0);

            var medicines = await query
                .Where(m => m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();

            return Ok(medicines);
        }

        [Authorize(Roles = "admin,staff,doctor")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Medicine>> GetMedicine(int id)
        {
            var medicine = await _context.Medicines
                .Include(m => m.StockHistory)
                .Include(m => m.Sales)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicine == null) return NotFound();
            return medicine;
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost]
        public async Task<ActionResult<Medicine>> PostMedicine(Medicine medicine)
        {
            medicine.CreatedAt = DateTime.UtcNow;
            medicine.UpdatedAt = DateTime.UtcNow;

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedicine), new { id = medicine.Id }, medicine);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicine(int id, Medicine medicine)
        {
            if (id != medicine.Id) return BadRequest();

            var dbMedicine = await _context.Medicines.FindAsync(id);
            if (dbMedicine == null) return NotFound();

            dbMedicine.Name = medicine.Name;
            dbMedicine.GenericName = medicine.GenericName;
            dbMedicine.Manufacturer = medicine.Manufacturer;
            dbMedicine.Description = medicine.Description;
            dbMedicine.Category = medicine.Category;
            dbMedicine.DosageForm = medicine.DosageForm;
            dbMedicine.Strength = medicine.Strength;
            dbMedicine.UnitPrice = medicine.UnitPrice;
            dbMedicine.ReorderLevel = medicine.ReorderLevel;
            dbMedicine.StorageConditions = medicine.StorageConditions;
            dbMedicine.RequiresPrescription = medicine.RequiresPrescription;
            dbMedicine.IsActive = medicine.IsActive;
            dbMedicine.UpdatedAt = DateTime.UtcNow;

            _context.Entry(dbMedicine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Medicines.AnyAsync(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();

            medicine.IsActive = false;
            medicine.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("{id}/stock")]
        public async Task<ActionResult<MedicineStock>> AddStock(int id, MedicineStock stock)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound("Medicine not found");

            stock.MedicineId = id;
            stock.TransactionDate = DateTime.UtcNow;

            _context.MedicineStocks.Add(stock);

            // Update medicine quantity
            if (stock.TransactionType == "Purchase")
            {
                medicine.QuantityInStock += stock.Quantity;
            }
            else if (stock.TransactionType == "Adjustment")
            {
                medicine.QuantityInStock = stock.Quantity;
            }

            medicine.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedicine), new { id = medicine.Id }, stock);
        }

        [Authorize(Roles = "staff")]
        [HttpPost("{id}/sale")]
        public async Task<ActionResult<MedicineSale>> RecordSale(int id, MedicineSale sale)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound("Medicine not found");

            if (medicine.QuantityInStock < sale.Quantity)
                return BadRequest("Insufficient stock");

            sale.MedicineId = id;
            sale.UnitPrice = medicine.UnitPrice;
            sale.TotalPrice = sale.Quantity * medicine.UnitPrice;
            sale.SaleDate = DateTime.UtcNow;

            _context.MedicineSales.Add(sale);

            // Update stock
            medicine.QuantityInStock -= sale.Quantity;
            medicine.UpdatedAt = DateTime.UtcNow;

            // Record stock transaction
            var stockTransaction = new MedicineStock
            {
                MedicineId = id,
                TransactionType = "Sale",
                Quantity = -sale.Quantity,
                UnitPrice = sale.UnitPrice,
                TransactionDate = DateTime.UtcNow,
                RecordedBy = sale.SoldBy,
                Notes = $"Sale to patient ID: {sale.PatientId}"
            };
            _context.MedicineStocks.Add(stockTransaction);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedicine), new { id = medicine.Id }, sale);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<Medicine>>> GetLowStockMedicines()
        {
            var medicines = await _context.Medicines
                .Where(m => m.IsActive && m.QuantityInStock <= m.ReorderLevel)
                .OrderBy(m => m.QuantityInStock)
                .ToListAsync();

            return Ok(medicines);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("expiring-soon")]
        public async Task<ActionResult<IEnumerable<Medicine>>> GetExpiringSoon([FromQuery] int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(days).ToString("yyyy-MM-dd");
            
            var medicines = await _context.Medicines
                .Where(m => m.IsActive && 
                           m.QuantityInStock > 0 && 
                           string.Compare(m.ExpiryDate, cutoffDate) <= 0)
                .OrderBy(m => m.ExpiryDate)
                .ToListAsync();

            return Ok(medicines);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("sales-report")]
        public async Task<ActionResult<object>> GetSalesReport(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = _context.MedicineSales
                .Include(ms => ms.Medicine)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(ms => ms.SaleDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(ms => ms.SaleDate <= toDate.Value);

            var report = new
            {
                TotalSales = await query.SumAsync(ms => ms.TotalPrice),
                TotalQuantitySold = await query.SumAsync(ms => ms.Quantity),
                NumberOfTransactions = await query.CountAsync(),
                TopSellingMedicines = await query
                    .GroupBy(ms => new { ms.MedicineId, ms.Medicine.Name })
                    .Select(g => new 
                    { 
                        MedicineName = g.Key.Name,
                        QuantitySold = g.Sum(ms => ms.Quantity),
                        Revenue = g.Sum(ms => ms.TotalPrice)
                    })
                    .OrderByDescending(x => x.QuantitySold)
                    .Take(10)
                    .ToListAsync()
            };

            return Ok(report);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet("inventory-value")]
        public async Task<ActionResult<object>> GetInventoryValue()
        {
            var medicines = await _context.Medicines
                .Where(m => m.IsActive && m.QuantityInStock > 0)
                .ToListAsync();

            var totalValue = medicines.Sum(m => m.QuantityInStock * m.UnitPrice);
            var totalItems = medicines.Sum(m => m.QuantityInStock);

            return Ok(new
            {
                TotalInventoryValue = totalValue,
                TotalItems = totalItems,
                NumberOfMedicines = medicines.Count
            });
        }
    }
}