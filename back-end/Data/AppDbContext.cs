using Microsoft.EntityFrameworkCore;
using back_end.Models;

namespace back_end.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Member> Members => Set<Member>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    
}