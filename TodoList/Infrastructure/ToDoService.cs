using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TodoList.Core.Dto;
using TodoList.Core.Entities;
using TodoList.Infrastructure.Database;

namespace TodoList.Infrastructure;

public class ToDoService(Data data, IHttpContextAccessor contextAccessor)
{
    private string GetLogin()
    {
        var login = contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(login))
        {
            throw new Exception("Email not found for the authenticated user.");
        }

        return login;
    }

    public async Task<List<GetToDoDto>> GetAllToDo()
    {
        string login = GetLogin();

        List<ToDo> todos = await data.ToDoS.ToListAsync();
        List<GetToDoDto> toDoDtos = todos.Where(x => x.Login == login).Select(x => new GetToDoDto()
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description
        }).ToList();

        return toDoDtos;
    }

    public async Task<GetToDoDto> GetToDoById(int id)
    {
        ToDo? todo = await data.ToDoS.FirstOrDefaultAsync(t => t.Id == id);
        GetToDoDto toDoDto = new()
        {
            Title = todo!.Title,
            Description = todo!.Description
        };
        return toDoDto!;
    }

    public async Task<GetToDoDto> AddToDo(AddToDoDto t)
    {
        string login = GetLogin();
        var todo = new ToDo()
        {
            Title = t.Title,
            Description = t.Description,
            Login = login
        };
        data.ToDoS.Add(todo);
        await data.SaveChangesAsync();

        GetToDoDto getToDoDto = new()
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description
        };
        
        return getToDoDto;
    }

    public async Task<string> UpdateToDo(UpdateToDoDto t)
    {
        ToDo? todo = await data.ToDoS.FirstOrDefaultAsync(t => t.Id == t.Id);
        if (todo == null) return "Not Found";
        todo.Title = t.Title;
        todo.Description = t.Description;
        data.ToDoS.Update(todo);
        await data.SaveChangesAsync();
        return "Updated";
    }

    public async Task<string> DeleteToDo(int id)
    {
        ToDo? todo = await data.ToDoS.FirstOrDefaultAsync(t => t.Id == t.Id);
        if (todo == null) return "Not Found";
        data.ToDoS.Remove(todo);
        await data.SaveChangesAsync();
        return "Deleted";
    }
}