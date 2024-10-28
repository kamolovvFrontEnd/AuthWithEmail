using Microsoft.EntityFrameworkCore;
using TodoList.Core.Entities;

namespace TodoList.Infrastructure.Database;

public class Data : DbContext
{
    public Data(DbContextOptions<Data> options) : base(options) { }

    public DbSet<ToDo> ToDoS { get; set; }
    public DbSet<IdentifyRequest> IdentifyRequests { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ToDo>()
            .HasKey(t => t.Id); // Указывает первичный ключ
        
        modelBuilder.Entity<ToDo>()
            .Property(t => t.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<ToDo>();
    }
}
