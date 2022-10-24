using System;
using FluentValidation;
using TodoApiWithControllers.Models;
namespace TodoApiWithControllers.Validators
{
    public class TodoItemDtoValidator : AbstractValidator<TodoItemDTO>
    {
        public TodoItemDtoValidator()
        {
            RuleFor(todoitemDto => todoitemDto.Name).NotEmpty().WithMessage("Name field is required");
            RuleFor(todoitemDto => todoitemDto.IsCompleted).NotEmpty().WithMessage("IsCompleted field is required");
        }
    }
}

