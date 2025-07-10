using Microsoft.EntityFrameworkCore;
using damiWeb.Models;

namespace damiWeb.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    public DbSet<OrderMaster> OrderMasters => Set<OrderMaster>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Item> Items => Set<Item>();
}
