using MailKit.Net.Smtp;
using MimeKit;

namespace TodoList.Infrastructure;

public class SendEmailService
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Code",
            "kmlvbilol@gmail.com")); // Замените на имя вашего приложения и email отправителя
        emailMessage.To.Add(new MailboxAddress("", email)); // Email получателя
        emailMessage.Subject = subject; // Тема письма
        emailMessage.Body = new TextPart("Plain") { Text = message }; // Содержимое письма

        var client = new SmtpClient();
        
        
        // Подключаемся к SMTP серверу (замените на ваши данные)
        await client.ConnectAsync("smtp.gmail.com", 465, true);
        client.AuthenticationMechanisms.Remove("OAUTH2");
        // Аутентификация на SMTP сервере
        await client.AuthenticateAsync("kmlvbilol@gmail.com", "ovulycglipmxirip");
        // Отправляем письмо
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
        client.Dispose();
    }
}