using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TodoList.Core.Dto;
using TodoList.Core.Entities;
using TodoList.Infrastructure.Database;

namespace TodoList.Infrastructure;

public class EmailService(
    SendEmailService sendEmailService,
    IConfiguration _configuration,
    Data data,
    IHttpContextAccessor _httpContextAccessor)
{
    public async Task SendEmailAsync(string email, string login, string password)
    {
        if (data.Users.Any(x => x.Login == login))
            throw new BadHttpRequestException("Такой пользователь уже существует");
        if (data.Users.Any(x => x.Email == email))
            throw new BadHttpRequestException("Пользователь с таким email уже существует");

        _httpContextAccessor.HttpContext.Session.SetString("Email", email);
        _httpContextAccessor.HttpContext.Session.SetString("Login", login);
        _httpContextAccessor.HttpContext.Session.SetString("Password", password);

        var code = new Random().Next(1000, 9999).ToString();

        data.IdentifyRequests.Add(new IdentifyRequest { Code = code });

        await data.SaveChangesAsync();

        await sendEmailService.SendEmailAsync(email, "Ваш код для входа", $"Ваш код: {code}");
    }

    public async Task<string> CheckCode(string code)
    {
        if (!data.IdentifyRequests.Any(x => x.Code == code))
            throw new BadHttpRequestException("Неверный пароль от email");

        string? _email = _httpContextAccessor.HttpContext.Session.GetString("Email");
        string? _login = _httpContextAccessor.HttpContext.Session.GetString("Login");
        string? _password = _httpContextAccessor.HttpContext.Session.GetString("Password");

        User user = new()
        {
            Email = _email!,
            Login = _login!,
            Password = _password!,
        };

        data.Users.Add(user);

        await data.SaveChangesAsync();

        string token = GenerateJwtToken(_login, _password);
        return token;
    }

    public async Task<string> Login(string login, string password)
    {
        if (!data.Users.Any(x => x.Login == login && x.Password == password))
            throw new BadHttpRequestException("Неверный логин или пароль");

        string token = GenerateJwtToken(login, password);
        return token;
    }

    private string GenerateJwtToken(string login, string password)
    {
        string? securityCode = _configuration["SecuritySetting:SecurityCode"];
        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(securityCode!); // Секретный ключ для подписи JWT

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, login),
            new Claim(ClaimTypes.PostalCode, password),
        };

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims), // Добавляем email как claim
            Expires = DateTime.UtcNow.AddHours(1), // Время жизни токена 1 час
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor); // Создаем токен
        return tokenHandler.WriteToken(token); // Возвращаем токен в строковом формате
    }
}