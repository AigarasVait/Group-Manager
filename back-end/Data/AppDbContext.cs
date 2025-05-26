using Microsoft.EntityFrameworkCore;
using back_end.Models;

namespace back_end.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    // DbSets represent tables in the database
        
        // Users table - stores application users
        public DbSet<User> Users => Set<User>();

        // Groups table - stores groups of users for shared transactions
        public DbSet<Group> Groups => Set<Group>();

        // Transactions table - stores financial transactions linked to groups
        public DbSet<Transaction> Transactions => Set<Transaction>();

        // DebtTrackers table - tracks debts between users inside groups
        public DbSet<DebtTracker> DebtTrackers => Set<DebtTracker>();
    
}