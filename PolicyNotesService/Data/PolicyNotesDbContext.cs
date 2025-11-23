using Microsoft.EntityFrameworkCore;
using PolicyNotesService.Models;

namespace PolicyNotesService.Data
{
    public class PolicyNotesDbContext : DbContext
    {
        public PolicyNotesDbContext(DbContextOptions<PolicyNotesDbContext> options)
            : base(options)
        {
        }

        //DTO object
        public DbSet<PolicyNote> PolicyNotes => Set<PolicyNote>();
    }
}
