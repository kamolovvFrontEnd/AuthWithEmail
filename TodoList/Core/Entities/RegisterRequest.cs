namespace TodoList.Core.Entities;

public class RegisterRequest
{
    public string Email { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}