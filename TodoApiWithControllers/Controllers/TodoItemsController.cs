using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApiWithControllers.Models;
using TodoApiWithControllers.Services;
using FluentValidation;
using FluentValidation.Results;

namespace TodoApiWithControllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoService _srv;
        private readonly IValidator<TodoItemDTO> _validator;

        public TodoItemsController(ITodoService srv, IValidator<TodoItemDTO> validator)
        {
            _srv = srv;
            _validator = validator;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return Ok(await _srv.GetTodoItems());
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItemDTO = await _srv.GetTodoItemById(id);

            if (todoItemDTO == null)
            {
                return NotFound();
            }

            return Ok(todoItemDTO);
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

            if(await _srv.UpdateTodoItem(id, todoItemDTO)) return NoContent();
            else return NotFound();
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

            if (await _srv.PartiallyUpdateTodoItem(id, todoItemUpdateDTO)) return NoContent();
            else return NotFound();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoItemDTO)
        {
            ValidationResult result = _validator.Validate(todoItemDTO);
            if (!result.IsValid)
            {
                var errors = new Dictionary<string, string>();
                foreach(var error in result.Errors)
                {
                    errors[error.PropertyName] = error.ErrorMessage;
                }
                System.Diagnostics.Debug.WriteLine(errors);

                return BadRequest(errors);
            }
            var createdTodoItem = await _srv.CreateTodoItem(todoItemDTO);

            if (createdTodoItem == null) return BadRequest();

            return CreatedAtAction(nameof(GetTodoItem), new { id = createdTodoItem.Id }, createdTodoItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            if (await _srv.DeleteTodoItem(id)) return NoContent();
            else return NotFound();
        }

    }
}
