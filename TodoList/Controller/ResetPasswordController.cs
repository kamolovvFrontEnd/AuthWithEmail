using Microsoft.AspNetCore.Mvc;
using TodoList.Infrastructure;

namespace TodoList.Controller;

[ApiController]
[Route("[controller]")]
public class ResetPasswordController(ResetPasswordService resetPasswordService) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpPost("SendEmail")]
    public async Task<IActionResult> SendEmailToResetPassword(string email)
    {
        await resetPasswordService.SendEmailForReset(email);
        return Ok();
    }

    [HttpPut("ResetPassword")]
    public async Task<IActionResult> ResetPassword(string code, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword) return BadRequest("Пароли не совпадают");

        return Ok(await resetPasswordService.NewPassword(code, newPassword));
    }
}