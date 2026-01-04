namespace CleanArchitectureApi.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
    Task SendEmailAsync(List<string> to, string subject, string body, bool isHtml = false);
    Task SendWelcomeEmailAsync(string to, string firstName);
    Task SendEmailConfirmationAsync(string to, string firstName, string confirmationLink);
    Task SendPasswordResetEmailAsync(string to, string firstName, string resetLink);
    Task SendNotificationEmailAsync(string to, string subject, string message);
}

