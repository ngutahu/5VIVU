using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace _5VIVU.Services
{
    public class SendMailService
    {
        public class MailSetting
        {
            public string Mail { get; set; }
            public string DisplayName { get; set; }
            public string Password { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
        }
        public class MailContent
        {
            public string To { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }

        }

        public class SendMailServices : IEmailSender
        {
            private readonly MailSetting mailSetting;
            private readonly ILogger<SendMailServices> logger;
            public SendMailServices(IOptions<MailSetting> _mailSetting, ILogger<SendMailServices> _logger)
            {
                mailSetting = _mailSetting.Value;
                logger = _logger;
                logger.LogInformation("Create SendMailService");
            }
            public async Task SendEmailAsync(string email, string subject, string htmlMessage)
            {
                var message = new MimeMessage();
                message.Sender = new MailboxAddress(mailSetting.DisplayName, mailSetting.Mail);
                message.From.Add(new MailboxAddress(mailSetting.DisplayName, mailSetting.Mail));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = htmlMessage;
                message.Body = builder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                try
                {
                    smtp.Connect(mailSetting.Host, mailSetting.Port, SecureSocketOptions.StartTls);
                    smtp.Authenticate(mailSetting.Mail, mailSetting.Password);
                    await smtp.SendAsync(message);
                }
                catch (Exception ex)
                {
                    System.IO.Directory.CreateDirectory("mailssave");
                    var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                    await message.WriteToAsync(emailsavefile);

                    logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
                    logger.LogError(ex.Message);
                }

                smtp.Disconnect(true);

                logger.LogInformation("send mail to: " + email);
            }
        }
    }
}
