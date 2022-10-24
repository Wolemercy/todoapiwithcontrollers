using System;
using System.Threading.Tasks;
using TodoApiWithControllers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TodoApiWithControllers.Services
{
    public class TodoService : ITodoService
    {
        private readonly TodoContext _context;

        public TodoService(TodoContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        // Get all TodoItems
        public async Task<IEnumerable<TodoItemDTO>> GetTodoItems()
        {
            var result = _context.Todo
                .Select(ele => ItemToDTO(ele));

            return await Task.FromResult(result.ToList());
        }

        // Get a TodoItem
        public async Task<TodoItemDTO?> GetTodoItemById(long id)
        {
            var todoItem = await _context.Todo.FindAsync(id);

            if (todoItem == null)
            {
                return null;
            }
            else return ItemToDTO(todoItem);
        }

        // create a TodoItem
        public async Task<TodoItemDTO?> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            var todoItem = new TodoItem
            {
                Name = todoItemDTO.Name,
                IsCompleted = todoItemDTO.IsCompleted
            };

            _context.Todo.Add(todoItem);
            await _context.SaveChangesAsync();

            return ItemToDTO(todoItem);
        }

        // update a TodoItem
        public async Task<bool> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            var todoitem = await _context.Todo.FindAsync(id);

            if (todoitem == null)
            {
                return false;
            }

            todoitem.Name = todoItemDTO.Name;
            todoitem.IsCompleted = todoItemDTO.IsCompleted;

            try
            {
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return false;
            }

            return true;
        }

        // partially update a TodoItem
        public async Task<bool> PartiallyUpdateTodoItem(long id, TodoItemUpdateDTO todoItemUpdateDTO)
        {
            var todoitem = await _context.Todo.FindAsync(id);

            if (todoitem == null)
            {
                return false;
            }

            if (todoItemUpdateDTO.Name != null) todoitem.Name = todoItemUpdateDTO.Name;
            if (todoItemUpdateDTO.IsCompleted != null) todoitem.IsCompleted = (bool)todoItemUpdateDTO.IsCompleted;

            try
            {
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return false;
            }

            return true;
        }


        // delete a TodoItem
        public async Task<bool> DeleteTodoItem(long id)
        {
            var todoItem = await _context.Todo.FindAsync(id);
            if (todoItem == null)
            {
                return false;
            }

            _context.Todo.Remove(todoItem);
            await _context.SaveChangesAsync();
            return true;
        }

        private bool TodoItemExists(long id)
        {
            return (_context.Todo?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static TodoItemDTO ItemToDTO(TodoItem todoItem)
        {
            return new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsCompleted = todoItem.IsCompleted
            };
        }
    }

    public interface ITodoService
    {
        Task<IEnumerable<TodoItemDTO>> GetTodoItems();
        Task<TodoItemDTO?> GetTodoItemById(long id);
        Task<TodoItemDTO?> CreateTodoItem(TodoItemDTO todoItemDTO);
        Task<bool> UpdateTodoItem(long id, TodoItemDTO todoItemDTO);
        Task<bool> PartiallyUpdateTodoItem(long id, TodoItemUpdateDTO todoItemUpdateDTO);
        Task<bool> DeleteTodoItem(long id);
    }
}

