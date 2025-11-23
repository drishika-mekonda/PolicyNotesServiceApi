using Microsoft.EntityFrameworkCore;
using PolicyNotesService.Data;
using PolicyNotesService.Models;

namespace PolicyNotesService.Repositories
{
    public class PolicyNoteRepository : IPolicyNoteRepository
    {
        private readonly PolicyNotesDbContext _context;

        public PolicyNoteRepository(PolicyNotesDbContext context)
        {
            _context = context;
        }

        public async Task<PolicyNote> AddAsync(PolicyNote note)
        {
            _context.PolicyNotes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task<List<PolicyNote>> GetAllAsync()
        {
            return await _context.PolicyNotes.AsNoTracking().ToListAsync();
        }

        public async Task<PolicyNote?> GetByIdAsync(int id)
        {
            return await _context.PolicyNotes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }
    }
}
