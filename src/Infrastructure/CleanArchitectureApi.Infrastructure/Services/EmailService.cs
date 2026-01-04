using CleanArchitectureApi.Application.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CleanArchitectureApi.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        await SendEmailAsync(new List<string> { to }, subject, body, isHtml);
    }

    public async Task SendEmailAsync(List<string> to, string subject, string body, bool isHtml = false)
    {
        try
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"] ?? throw new InvalidOperationException("SMTP Server not configured");
            var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
            var smtpUsername = emailSettings["SmtpUsername"] ?? throw new InvalidOperationException("SMTP Username not configured");
            var smtpPassword = emailSettings["SmtpPassword"] ?? throw new InvalidOperationException("SMTP Password not configured");
            var fromEmail = emailSettings["FromEmail"] ?? throw new InvalidOperationException("From Email not configured");
            var fromName = emailSettings["FromName"] ?? "Clean Architecture API";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));

            foreach (var recipient in to)
            {
                message.To.Add(new MailboxAddress("", recipient));
            }

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUsername, smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", to));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipients}", string.Join(", ", to));
            throw;
        }
    }

    public async Task SendWelcomeEmailAsync(string to, string firstName)
    {
        var subject = "Welcome to Clean Architecture API!";
        var body = $@"
            <html>
            <body>
                <h2>Welcome, {firstName}!</h2>
                <p>Thank you for joining Clean Architecture API. We're excited to have you on board!</p>
                <p>Your account has been successfully created and you can now start using our services.</p>
                <p>If you have any questions, please don't hesitate to contact our support team.</p>
                <br>
                <p>Best regards,<br>The Clean Architecture API Team</p>
            </body>
            </html>";

        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendEmailConfirmationAsync(string to, string firstName, string confirmationLink)
    {
        var subject = "Confirm Your Email Address";
        var body = $@"
            <html>
            <body>
                <h2>Email Confirmation</h2>
                <p>Hello {firstName},</p>
                <p>Please confirm your email address by clicking the link below:</p>
                <p><a href=""{confirmationLink}"" style=""background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;"">Confirm Email</a></p>
                <p>If you didn't create an account with us, please ignore this email.</p>
                <br>
                <p>Best regards,<br>The Clean Architecture API Team</p>
            </body>
            </html>";

        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendPasswordResetEmailAsync(string to, string firstName, string resetLink)
    {
        var subject = "Password Reset Request";
        var body = $@"
            <html>
            <body>
                <h2>Password Reset</h2>
                <p>Hello {firstName},</p>
                <p>You have requested to reset your password. Click the link below to reset it:</p>
                <p><a href=""{resetLink}"" style=""background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;"">Reset Password</a></p>
                <p>This link will expire in 24 hours for security reasons.</p>
                <p>If you didn't request a password reset, please ignore this email.</p>
                <br>
                <p>Best regards,<br>The Clean Architecture API Team</p>
            </body>
            </html>";

        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendNotificationEmailAsync(string to, string subject, string message)
    {
        var body = $@"
            <html>
            <body>
                <h2>Notification</h2>
                <p>{message}</p>
                <br>
                <p>Best regards,<br>The Clean Architecture API Team</p>
            </body>
            </html>";

        await SendEmailAsync(to, subject, body, true);
    }
}

