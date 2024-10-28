using Microsoft.AspNetCore.Mvc;
using TodoList.Core.Entities;
using TodoList.Infrastructure;

namespace TodoList.Controller;

public class SendEmailController(EmailService emailService)
    : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        await emailService.SendEmailAsync(request.Email, request.Login, request.Password);
        return Ok();
    }
    
    [HttpPost("CheckCode")]
    public async Task<IActionResult> CheckCode(string code)
    {
        return Ok(await emailService.CheckCode(code));
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(string login, string password)
    {
        return Ok(await emailService.Login(login, password));
    }
}