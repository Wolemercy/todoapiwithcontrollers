using System;
namespace TodoApiWithControllers.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public bool? Secret { get; set; }
    }

    public class TodoItemDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class TodoItemUpdateDTO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool? IsCompleted { get; set; }
    }
}

