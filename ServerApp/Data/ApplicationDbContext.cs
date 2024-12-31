using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    
    public DbSet<User> Users { get; set; }
}