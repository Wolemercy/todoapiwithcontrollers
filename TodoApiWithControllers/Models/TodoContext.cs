using System;
using Microsoft.EntityFrameworkCore;

namespace TodoApiWithControllers.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> Todo { get; set; } = null!;
    }
}

