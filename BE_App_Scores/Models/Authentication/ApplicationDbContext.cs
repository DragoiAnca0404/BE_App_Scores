using BE_App_Scores.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Echipe> Echipe { get; set; }
    public DbSet<CreareEchipe> CreareEchipe { get; set; }
    public DbSet<Activitate> Activitati { get; set; }
    public DbSet<GestionareMeci> GestionareMeciuri { get; set; }

    public DbSet<Meci> Meci { get; set; }

    public DbSet<Scoruri> Scoruri { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        SeedRoles(builder);


        // Configurare Echipe
        builder.Entity<Echipe>()
            .HasMany(e => e.CreareEchipe)
            .WithOne(ce => ce.Echipe)
            .HasForeignKey(ce => ce.EchipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurare CreareEchipe
        builder.Entity<CreareEchipe>()
            .HasKey(ce => new { ce.UserId, ce.EchipeId });

        builder.Entity<CreareEchipe>()
            .HasOne(ce => ce.User)
            .WithMany()
            .HasForeignKey(ce => ce.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CreareEchipe>()
            .HasOne(ce => ce.Echipe)
            .WithMany(e => e.CreareEchipe)
            .HasForeignKey(ce => ce.EchipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurare Activitate
        builder.Entity<Activitate>()
            .HasMany(a => a.GestionareMeciuri)
            .WithOne(gm => gm.Activitate)
            .HasForeignKey(gm => gm.IdActivitate)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurare GestionareMeci
        builder.Entity<GestionareMeci>()
                .HasKey(gm => new { gm.IdActivitate, gm.IdEchipa, gm.IdMeci, gm.IdScor });

        builder.Entity<GestionareMeci>()
            .HasOne(gm => gm.Activitate)
            .WithMany(a => a.GestionareMeciuri)
            .HasForeignKey(gm => gm.IdActivitate)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GestionareMeci>()
            .HasOne(gm => gm.Echipa)
            .WithMany(e => e.GestionareMeciuri)
            .HasForeignKey(gm => gm.IdEchipa)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurare Meci
        builder.Entity<Meci>()
            .HasMany(e => e.GestionareMeciuri)
            .WithOne(ce => ce.Meci)
            .HasForeignKey(ce => ce.IdMeci)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurare Echipe
        builder.Entity<Scoruri>()
            .HasMany(e => e.GestionareMeciuri)
            .WithOne(ce => ce.Scoruri)
            .HasForeignKey(ce => ce.IdScor)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "1" },
            new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER", ConcurrencyStamp = "2" },
            new IdentityRole { Id = "3", Name = "HR", NormalizedName = "HR", ConcurrencyStamp = "3" }
        );
    }
}
