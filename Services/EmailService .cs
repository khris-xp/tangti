using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;

namespace tangti.Services;

public class EmailService
{


    private readonly string smtpHost;
    private readonly int smtpPort;
    private readonly string smtpUsername;
    private readonly string smtpPassword;
    private readonly bool enableSsl;
        
    public EmailService(IConfiguration configuration,IOptions<MailSettings> mailSettings)
    {

        smtpHost = mailSettings.Value.Host;
        smtpPort = mailSettings.Value.Port;
        smtpUsername = mailSettings.Value.Username;
        smtpPassword = mailSettings.Value.Password;
        enableSsl = mailSettings.Value.EnableSsl;
    }


     public async Task<bool> SendEmail(string toAddress, string subject, string body)
    {


        var smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = true
        };


        var mailMessage = new MailMessage("teawkrub.ghs@gmail.com",toAddress);
        mailMessage.Subject = subject;
        mailMessage.Body = body;
        mailMessage.IsBodyHtml = true;
        Console.WriteLine("Sending email...");
        Console.WriteLine("From: " + mailMessage.From);
        Console.WriteLine("To: " + mailMessage.To);
        Console.WriteLine("Subject: " + mailMessage.Subject);
        Console.WriteLine("Body: " + mailMessage.Body);
        
        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine("Email sent successfully!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to send email. Error: " + ex.Message);
            return false;
        }
    }

}

