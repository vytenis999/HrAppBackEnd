using Microsoft.EntityFrameworkCore;
using MouseTagProject.Context;
using MouseTagProject.Models;

namespace MouseTagProject.Repository
{
    public class StatusRepository
    {
        private readonly MouseTagProjectContext _context;

        public StatusRepository(MouseTagProjectContext context)
        {
            _context = context;
        }

        public async Task<List<Status>> GetStatusesAsync()
        {
            return await _context.Statuses.ToListAsync();
        }
    }
}
