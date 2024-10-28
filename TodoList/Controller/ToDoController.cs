using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Core.Dto;
using TodoList.Core.Entities;
using TodoList.Infrastructure;

namespace TodoList.Controller;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ToDoController(ToDoService service) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet("GetAllToDo")]
    public async Task<ActionResult> GetAllToDo()
    {
        return Ok(await service.GetAllToDo());
    }

    [HttpGet("GetToDoById")]
    public async Task<ActionResult> GetToDoById(int id)
    {
        return Ok(await service.GetToDoById(id));
    }

    [HttpPost("AddToDo")]
    public async Task<ActionResult> AddToDo(AddToDoDto t)
    {
        return Ok(await service.AddToDo(t));
    }

    [HttpPut("UpdateToDo")]
    public async Task<ActionResult> UpdateToDo(UpdateToDoDto t)
    {
        return Ok(await service.UpdateToDo(t));
    }

    [HttpDelete("DeleteToDo")]
    public async Task<ActionResult> DeleteToDo(int id)
    {
        return Ok(await service.DeleteToDo(id));
    }
}