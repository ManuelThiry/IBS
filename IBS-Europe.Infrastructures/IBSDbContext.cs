using IBS_Europe.Infrastructures.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IBS_Europe.Infrastructures;

public class IBSDbContext : IdentityDbContext
{
    public IBSDbContext(DbContextOptions<IBSDbContext> options) : base(options)
    {
    }

    public DbSet<Products> Products { get; set; }
    public DbSet<People> People { get; set; }
    public DbSet<Partners> Partners { get; set; }
    
    public DbSet<Broker> Brokers { get; set; }
    
    public DbSet<Email> Email { get; set; }
    
    public DbSet<Informations> Informations { get; set; }
    
    public DbSet<Translator> Translator { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Broker>()
            .HasOne(b => b.Products)
            .WithMany(p => p.Brokers)
            .HasForeignKey(b => b.ProductsId);


        modelBuilder.Entity<Products>()
            .Property(p => p.Name)
            .IsRequired();
        
        modelBuilder.Entity<Products>()
            .Property(p => p.Text)
            .IsRequired();
        
        modelBuilder.Entity<People>()
            .Property(p => p.FirstName)
            .IsRequired();
        
        modelBuilder.Entity<People>()
            .Property(p => p.Priority)
            .IsRequired();
        
        modelBuilder.Entity<People>()
            .Property(p => p.Email)
            .IsRequired();
        
        modelBuilder.Entity<People>()
            .Property(p => p.Role)
            .IsRequired();
        
        modelBuilder.Entity<Partners>()
            .Property(p => p.Name)
            .IsRequired();
        
        modelBuilder.Entity<Partners>()
            .Property(p => p.Priority)
            .IsRequired();
        
        modelBuilder.Entity<Broker>()
            .Property(p => p.Name)
            .IsRequired();
        
        modelBuilder.Entity<Broker>()
            .Property(p => p.Category)
            .IsRequired();
        
        modelBuilder.Entity<Broker>()
            .Property(p => p.Path)
            .IsRequired();
        
        modelBuilder.Entity<Broker>()
            .Property(p => p.Priority)
            .IsRequired();
        
        modelBuilder.Entity<Email>()
            .Property(p => p.Name)
            .IsRequired();
        
        modelBuilder.Entity<Email>()
            .Property(p => p.Description)
            .IsRequired();
        
        modelBuilder.Entity<Email>()
            .Property(p => p.EmailAddress)
            .IsRequired();
        
        modelBuilder.Entity<Informations>()
            .Property(p => p.Text)
            .IsRequired();
        
        modelBuilder.Entity<Informations>()
            .Property(p => p.Description)
            .IsRequired();
        
        modelBuilder.Entity<Informations>()
            .Property(p => p.Priority)
            .IsRequired();
        
        modelBuilder.Entity<Translator>()
            .Property(p => p.Text)
            .IsRequired();
        
        modelBuilder.Entity<Translator>()
            .Property(p => p.IsChecked)
            .HasDefaultValue(0);


    }

}