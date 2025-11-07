using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SkillSwap.Interfaces;

namespace SkillSwap.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Correo para restaurar contraseña
        public async Task SendPasswordResetEmail(string toEmail, string resetLink)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("SkillSwap", "no-reply@miapp.com"));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = "Restauración de Contraseña SkillSwap";

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link href=""https://fonts.googleapis.com/css2?family=Urbanist:wght@400;500;600;700&display=swap"" rel=""stylesheet"">
</head>
<body style=""margin:0;padding:0;font-family:'Urbanist',sans-serif;background-color:#f5f5f5;"">
    <table role=""presentation"" style=""width:100%;background-color:#f5f5f5;border-collapse:collapse;"">
        <tr>
            <td align=""center"" style=""padding:40px 20px;"">
                <table role=""presentation"" style=""max-width:600px;width:100%;background-color:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08);"">

                    <!-- Header -->
                    <tr>
                        <td style=""background:linear-gradient(135deg,#F0AC27 0%,#da731e 50%,#ea2424 100%);height:180px;"">
                            <table style=""width:100%;height:180px;"">
                                <tr>
                                    <td align=""center"">
                                        <div style=""background-color:rgba(255,255,255,0.95);padding:16px 32px;border-radius:50px;box-shadow:0 8px 32px rgba(0,0,0,0.12);"">
                                            <h1 style=""margin:0;font-size:32px;font-weight:700;letter-spacing:-0.5px;
                                                background:linear-gradient(90deg,#F0AC27 0%,#da731e 60%,#ea2424 100%);
                                                -webkit-background-clip:text;-webkit-text-fill-color:transparent;"">
                                                SkillSwap
                                            </h1>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Content -->
                    <tr>
                        <td style=""padding:48px 40px;"">
                            <h2 style=""margin:0 0 24px 0;font-size:28px;font-weight:700;text-align:center;
                                background:linear-gradient(90deg,#F0AC27 0%,#da731e 60%,#ea2424 100%);
                                -webkit-background-clip:text;-webkit-text-fill-color:transparent;letter-spacing:-0.5px;"">
                                Dear SkillSwap User
                            </h2>

                            <p style=""font-size:16px;line-height:1.7;color:#374151;text-align:center;"">
                                We have received a request to reset the password for your SkillSwap account.
                                If you didn't make this request, you can safely ignore this email.
                            </p>

                            <p style=""font-size:16px;line-height:1.7;color:#374151;text-align:center;"">
                                To reset your password, simply click the button below:
                            </p>

                            <!-- Button -->
                            <table style=""width:100%;"">
                                <tr>
                                    <td align=""center"">
                                        <a href=""https://skillswapten.vercel.app/auth/password/reset/{resetLink}""
                                           style=""display:inline-block;padding:16px 48px;font-size:16px;font-weight:600;color:#fff;
                                           background:linear-gradient(90deg,#F0AC27 0%,#da731e 60%,#ea2424 100%);
                                           text-decoration:none;border-radius:50px;box-shadow:0 4px 16px rgba(234,36,36,0.25);"">
                                            Reset Your Password
                                        </a>
                                    </td>
                                </tr>
                            </table>

                            <!-- Security Notice -->
                            <div style=""background-color:#FEF3C7;border-left:4px solid #F59E0B;padding:16px 20px;border-radius:8px;margin-top:24px;"">
                                <p style=""margin:0;font-size:14px;color:#92400E;"">
                                    <strong>Security Notice:</strong> This link will expire in 24 hours for your protection.
                                </p>
                            </div>

                            <p style=""font-size:14px;color:#6B7280;text-align:center;margin-top:32px;"">
                                Need help? Contact our support team at 
                                <a href=""mailto:skillswapten@gmail.com"" style=""color:#6B7280;font-weight:600;text-decoration:none;"">
                                    skillswapten@gmail.com
                                </a>
                            </p>

                            <div style=""height:1px;background:linear-gradient(90deg,transparent,#E5E7EB 50%,transparent);margin:32px 0;""></div>

                            <p style=""font-size:14px;color:#6B7280;text-align:center;"">
                                Thank you for being part of SkillSwap!<br/><br/>
                                <span style=""background:linear-gradient(90deg,#F0AC27 0%,#da731e 60%,#ea2424 100%);
                                -webkit-background-clip:text;-webkit-text-fill-color:transparent;"">The SkillSwap Team</span>
                            </p>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style=""padding:24px 40px;background-color:#F9FAFB;border-top:1px solid #E5E7EB;"">
                            <p style=""margin:0;font-size:12px;color:#9CA3AF;text-align:center;"">
                                © 2025 SkillSwap. All rights reserved.<br/>This is an automated message, please do not reply.
                            </p>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration["SMTP_HOST"], int.Parse(_configuration["SMTP_PORT"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_configuration["SMTP_USER"], _configuration["SMTP_PASS"]);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }

        // Correo de bienvenida al registrarse
        public async Task SendWelcomeEmail(string toEmail, string userName)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("SkillSwap", "no-reply@miapp.com"));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = "¡Bienvenido a SkillSwap!";

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <link href=""https://fonts.googleapis.com/css2?family=Urbanist:wght@400;500;600;700&display=swap"" rel=""stylesheet"" />
</head>
<body style=""margin:0;padding:0;font-family:'Urbanist',sans-serif;background-color:#f5f5f5;"">
    <table role=""presentation"" style=""width:100%;border-collapse:collapse;background-color:#f5f5f5;"">
        <tr>
            <td align=""center"" style=""padding:40px 20px;"">
                <table role=""presentation"" style=""max-width:600px;background-color:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08);"">
                    
                    <!-- Header -->
                    <tr>
                        <td style=""background:linear-gradient(135deg,#F0AC27 0%,#da731e 50%,#ea2424 100%);height:180px;"">
                            <table style=""width:100%;height:180px;"">
                                <tr>
                                    <td align=""center"">
                                        <div style=""background-color:rgba(255,255,255,0.95);padding:16px 32px;border-radius:50px;box-shadow:0 8px 32px rgba(0,0,0,0.12);"">
                                            <h1 style=""font-size:32px;font-weight:700;
                                                background:linear-gradient(90deg,#F0AC27 0%,#da731e 60%,#ea2424 100%);
                                                -webkit-background-clip:text;-webkit-text-fill-color:transparent;"">
                                                SkillSwap
                                            </h1>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Content -->
                    <tr>
                        <td style=""padding:48px 40px;"">
                            <h2 style=""text-align:center;font-size:26px;font-weight:700;
                                background:linear-gradient(90deg,#F0AC27 0%,#da731e 60%,#ea2424 100%);
                                -webkit-background-clip:text;-webkit-text-fill-color:transparent;"">
                                ¡Bienvenido a SkillSwap, {userName}!
                            </h2>

                            <p style=""font-size:16px;color:#374151;text-align:center;"">
                                Nos alegra que formes parte de nuestra comunidad de intercambio de habilidades digitales.
                                En SkillSwap, creemos que el conocimiento crece cuando se comparte.
                            </p>

                            <p style=""font-size:16px;color:#374151;text-align:center;"">
                                Explora, aprende y enseña con seguridad. Estamos aquí para ayudarte a conectar con otros talentos.
                            </p>

                            <table role=""presentation"" style=""width:100%;margin-top:24px;"">
                                <tr>
                                    <td align=""center"">
                                        <a href=""https://skillswapten.vercel.app""
                                            style=""display:inline-block;padding:16px 48px;font-size:16px;font-weight:600;color:#fff;
                                            background:linear-gradient(90deg,#F0AC27 0%,#da731e 60%,#ea2424 100%);
                                            text-decoration:none;border-radius:50px;box-shadow:0 4px 16px rgba(234,36,36,0.25);"">
                                            Ir a SkillSwap
                                        </a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style=""padding:24px 40px;background-color:#F9FAFB;border-top:1px solid #E5E7EB;"">
                            <p style=""margin:0;font-size:12px;line-height:1.5;color:#9CA3AF;text-align:center;"">
                                © 2025 SkillSwap. Todos los derechos reservados.<br/>
                                Este es un mensaje automático, por favor no respondas a este correo.
                            </p>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration["SMTP_HOST"], int.Parse(_configuration["SMTP_PORT"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_configuration["SMTP_USER"], _configuration["SMTP_PASS"]);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}
