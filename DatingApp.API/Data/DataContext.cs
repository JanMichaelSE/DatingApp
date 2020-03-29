using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore; // (DevNote): Downloaded with NuggetPacketManager


namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        //This is the table name
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }

    }
}