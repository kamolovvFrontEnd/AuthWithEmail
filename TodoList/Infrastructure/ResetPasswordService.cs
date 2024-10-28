using TodoList.Core.Entities;
using TodoList.Infrastructure.Database;

namespace TodoList.Infrastructure;

public class ResetPasswordService(SendEmailService sendEmailService, Data data, IHttpContextAccessor httpContextAccessor)
{
    public async Task SendEmailForReset(string email)
    {
        httpContextAccessor.HttpContext.Session.SetString("Email", email);

        var code = new Random().Next(1000, 9999).ToString();

        data.IdentifyRequests.Add(new IdentifyRequest() { Code = code });

        await data.SaveChangesAsync();

        await sendEmailService.SendEmailAsync(email, "Ваш код для входа", $"Ваш код: {code}");
    }

    public async Task<string> NewPassword(string code, string newPassword)
    {
        if (!data.IdentifyRequests.Any(x => x.Code == code))
            throw new("Невеный пароль");

        var email = httpContextAccessor.HttpContext.Session.GetString("Email");

        var user = data.Users.FirstOrDefault(x => x.Email == email);

        if (user == null) return "Not Found";

        user.Password = newPassword;

        data.Users.Update(user);

        await data.SaveChangesAsync();

        return "Password updated";
    }
}