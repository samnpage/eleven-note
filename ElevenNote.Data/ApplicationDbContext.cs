using ElevenNote.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElevenNote.Data;

public class ApplicationDbContext : IdentityDbContext<UserEntity, IdentityRole<int>, int>
{
    // Constructor that passes in options that apply to dbcontext that pulls from program.cs. (passing it the connection string.)
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<NoteEntity> Notes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>().ToTable("Users");
    }

    // public DbSet<UserEntity> Users { get; set; } = null!;
}