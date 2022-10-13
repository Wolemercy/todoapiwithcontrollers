using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApiWithControllers.Models;

namespace TodoApiWithControllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodo()
        {
            return await _context.Todo
                .Select(ele => ItemToDTO(ele))
                .ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _context.Todo.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(todoItem);
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                return BadRequest();
            }

            var todoitem = await _context.Todo.FindAsync(id);

            if (todoitem == null)
            {
                return NotFound();
            }

            todoitem.Name = todoItemDTO.Name;
            todoitem.IsCompleted = todoItemDTO.IsCompleted;

            try
            {
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // PATCH: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTodoItem(long id, TodoItemUpdateDTO todoItemUpdateDTO)
        {
            if (id != todoItemUpdateDTO.Id)
            {
                return BadRequest();
            }

            var todoitem = await _context.Todo.FindAsync(id);

            if (todoitem == null)
            {
                return NotFound();
            }

            if (todoItemUpdateDTO.Name != null) todoitem.Name = todoItemUpdateDTO.Name;
            if (todoItemUpdateDTO.IsCompleted != null) todoitem.IsCompleted = (bool)todoItemUpdateDTO.IsCompleted;

            try
            {
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoItemDTO)
        {
            var todoItem = new TodoItem
            {
                Name = todoItemDTO.Name,
                IsCompleted = todoItemDTO.IsCompleted
            };

            _context.Todo.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, ItemToDTO(todoItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.Todo.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.Todo.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
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
}
