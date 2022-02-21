using System;
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
    public class RequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestsController(AppDbContext context)
        {
            _context = context;
        }



        // CUSTOM METHODS

        // gets all requests in review status
        // GET: api/requests/review/id
        [HttpGet("review/{userId}")] 
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsAsReview(int userId) {
            var requests = await _context.Requests
                            .Where(x => x.Status == "REVIEW"
                            && x.Id != userId)
                            .ToListAsync();
            return requests;
        }

        // sets reviewed if total greater than 51
        // PUT: api/requests/id/review
        [HttpPut("{id}/request")]
        public async Task<ActionResult<Request>> SetReview(int userId, Request request) {

            if (request.Total <= 50) {
                request.Status = "APPROVED";
            } else {
                request.Status = "REVIEW";
            }

            _context.Entry(request).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();
            }

        // sets approved
        // PUT: api/requests/id/setApproved
        [HttpPut("{id}/setApproved")]
        public async Task<ActionResult<Request>> SetApproved(int userId, Request request) {
            request.Status = "APPROVED";

            _context.Entry(request).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // sets rejected
        // PUT: api/requests/id/setRejected
        [HttpPut("{id}/setRejected")]
        public async Task<ActionResult<Request>> SetRejected(int userId, Request request) {
            request.Status = "REJECTED";

            _context.Entry(request).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        } // do we need to add more code to force the user to add a rejection reason? when a request is rejected?
        // do we need to add more code to deny the user from making changes to the total/status?





        //_____________________________________________________________________________________________________________________________________________________

        // GENERATED METHODS

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            return await _context.Requests
                                    .Include(x => x.User)
                                    .Include(p => p.RequestLines)
                                    .ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id)
        {
            var request = await _context.Requests//.FindAsync(id);
                                            .Include(x => x.User)
                                            .Include(p => p.RequestLines)
                                            .SingleOrDefaultAsync(x => x.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            return request;
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
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

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request)
        {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
