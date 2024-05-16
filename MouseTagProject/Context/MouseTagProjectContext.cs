using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MouseTagProject.Models;

namespace MouseTagProject.Context
{
    public class MouseTagProjectContext : IdentityDbContext<ApplicationUser>
    {
        public MouseTagProjectContext(DbContextOptions options) : base(options) { }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Technology> Technologies { get; set; }
        public DbSet<UserDate> UserDates { get; set; }
        public DbSet<CandidateTechnology> CandidateTechnologies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientCandidate> ClientCandidates { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Status> Statuses { get; set; }
    }
}
