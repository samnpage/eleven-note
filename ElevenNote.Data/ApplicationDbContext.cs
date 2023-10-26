using ElevenNote.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElevenNote.Data;

public class ApplicationDbContext : DbContext
{
    // Constructor that passes in options that apply to dbcontext that pulls from program.cs. (passing it the connection string.)
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users { get; set; } = null!;
}