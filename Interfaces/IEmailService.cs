namespace SkillSwap.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmail(string toEmail, string resetToken);
    Task SendWelcomeEmail(string toEmail, string userName);
}