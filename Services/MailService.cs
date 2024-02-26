using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

public class MailOptions {
    public string? Mail {set; get;}
    public string? DisplayName {set; get;}
    public string? Password {set; get;}
    public string? Host {set; get;}
    public int Port {set; get;}
}



public class SendMail : IEmailSender
{
    private readonly MailOptions mailOptions;
    public SendMail(IOptions<MailOptions> mailOptions){
        this.mailOptions = mailOptions.Value;
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.Sender = new MailboxAddress(mailOptions.DisplayName, mailOptions.Mail);
        message.From.Add(new MailboxAddress(mailOptions.DisplayName, mailOptions.Mail));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;

        var body = new BodyBuilder();
        body.HtmlBody = htmlMessage;

        message.Body = body.ToMessageBody();

        using var sendMail = new SmtpClient();
        try
        {
            await sendMail.ConnectAsync(mailOptions.Host, mailOptions.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await sendMail.AuthenticateAsync(mailOptions.Mail, mailOptions.Password);
            Directory.CreateDirectory("MailSave");
            var emailSaveFile = string.Format(@"MailSave/{0}.eml", Guid.NewGuid());
            await message.WriteToAsync(emailSaveFile);
            await sendMail.SendAsync(message);
        }
        catch (System.Exception)
        {
            Directory.CreateDirectory("MailSave");
            var emailSaveFile = string.Format(@"MailSave/{0}.eml", Guid.NewGuid());
            await message.WriteToAsync(emailSaveFile);
            throw;
        }

        await sendMail.DisconnectAsync(true);
    }
}