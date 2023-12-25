using Microsoft.EntityFrameworkCore;

namespace backend;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<EduDocs> EduDocs { get; set; }
    public DbSet<HFInfo> HfInfos { get; set; }
    public DbSet<Institut> Instituts { get; set; }
    public DbSet<PersonalData> PersonalDatas { get; set; }
    public DbSet<Work> Works { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=example");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("lab3"); // Set your schema name here

        // Specify schema for each entity/table
        modelBuilder.Entity<EduDocs>().ToTable("EduDocs", schema: "lab3");
        modelBuilder.Entity<HFInfo>().ToTable("HfInfos", schema: "lab3");
        modelBuilder.Entity<Institut>().ToTable("Instituts", schema: "lab3");
        modelBuilder.Entity<PersonalData>().ToTable("PersonalDatas", schema: "lab3");
        modelBuilder.Entity<Work>().ToTable("Works", schema: "lab3");

        // Other model configurations, if any

        base.OnModelCreating(modelBuilder);
    }

}
