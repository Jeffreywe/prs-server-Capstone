using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prs_server.Models;

namespace prs_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendorsController(AppDbContext context) {
            _context = context;
        }

        //// Mostly Finished? not returning anything tho
        //[HttpGet("po/{vendorId}")]
        //public async Task<ActionResult<IEnumerable<Vendor>>> PO(int vendorId) {
        //    var vendor = await _context.Vendors
        //                            .FindAsync(vendorId);

        //    var bob = (from v in _context.Vendors
        //               join p in _context.Products
        //               on v.Id equals p.VendorId
        //               join rl in _context.RequestLines
        //               on p.Id equals rl.ProductId
        //               join r in _context.Requests
        //               on rl.RequestId equals r.Id
        //               where r.Status == "APPROVED"
        //               select new {
        //                   p, rl, r
        //               });
        //    return new List<Vendor>();
        //}
        

        [HttpGet("po/{vendorId}")]
        public async Task<ActionResult<Po>> CreatePo(int vendorId) {
            var vendor = await _context.Vendors.FindAsync(vendorId);//SingleOrDefaultAsync(x => x.Id == vendorId);

            var bob = (from v in _context.Vendors
                      join p in _context.Products
                      on v.Id equals p.VendorId
                      join rl in _context.RequestLines
                      on p.Id equals rl.ProductId
                      join r in _context.Requests
                      on rl.RequestId equals r.Id
                      where r.Status == "APPROVED"
                      && v.Id == vendorId
                      select new {
                          p.Id,
                          Product = p.Name,
                          rl.Quantity,
                          p.Price,
                          LineTotal = p.Price * rl.Quantity
                      });

            var sortedLines = new SortedList<int, Poline>();
            foreach(var bo in bob) {
                if (!sortedLines.ContainsKey(bo.Id)) {
                    var poline = new Poline() {
                        Product = bo.Product,
                        Quantity = 0,
                        Price = bo.Price,
                        LineTotal = bo.LineTotal
                    };
                    sortedLines.Add(bo.Id, poline);
                }
                sortedLines[bo.Id].Quantity += bo.Quantity;
            }
            
            var pizza = sortedLines.Values.Sum(x => x.LineTotal);

            var pizzalist = new Po() {
                Vendor = vendor,
                Polines = sortedLines.Values,
                PoTotal = pizza
            };
            return pizzalist;
        }









        // GET: api/Vendors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendors() {
            return await _context.Vendors
                                    .Include(x => x.Products)
                                    .ToListAsync();
        }


        // GET: api/Vendors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(int id)
        {
            var vendor = await _context.Vendors
                                            .Include(x => x.Products)
                                            .SingleOrDefaultAsync(x => x.Id == id);

            if (vendor == null)
            {
                return NotFound();
            }

            return vendor;
        }

        // PUT: api/Vendors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendor(int id, Vendor vendor)
        {
            if (id != vendor.Id)
            {
                return BadRequest();
            }

            _context.Entry(vendor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendorExists(id))
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

        // POST: api/Vendors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vendor>> PostVendor(Vendor vendor)
        {
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVendor", new { id = vendor.Id }, vendor);
        }

        // DELETE: api/Vendors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VendorExists(int id)
        {
            return _context.Vendors.Any(e => e.Id == id);
        }
    }
}
