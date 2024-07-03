namespace ctrlAltDefeatApp.Data.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using ctrlAltDefeatApp.Data.Entities;

public class CtrlAltDefeatDatabaseContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public CtrlAltDefeatDatabaseContext(IConfiguration configuration) { 
        Configuration = configuration; 
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("SqliteDB"));
    }


    protected override void OnModelCreating (ModelBuilder modelBuilder)
    {            
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<User>().HasData(
            new User { Id = Guid.NewGuid(), Username = "admin", Password = "admin", Level = "N00b", CurrentXP = 0, currentAverage = 0, ConfirmNewXP=false},
            new User { Id = Guid.NewGuid(), Username = "Schätzmeister", Password = "admin", Level = "Fortgeschrittener", CurrentXP = 3200, currentAverage = 89.9, ConfirmNewXP=false},
            new User { Id = Guid.NewGuid(), Username = "Praktikantenmann", Password = "admin", Level = "Anfänger", CurrentXP = 20, currentAverage = 10.9, ConfirmNewXP=false},
            new User { Id = Guid.NewGuid(), Username = "Jürgen", Password = "admin", Level = "Wunderkind", CurrentXP = 39100, currentAverage = 27.9, ConfirmNewXP=false}
            );
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Estimates> Estimates { get; set; }
}

