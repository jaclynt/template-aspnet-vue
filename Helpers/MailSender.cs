using System.Net.Mail;
using System.Net;

namespace ASPNetVueTemplate.Helpers;

public class MailSender
{
	public SmtpClient SmtpClient { get; set; }
	public string FromEmail { get; set; }

	public async Task SendAsync(MailMessage message)
	{
		message.From = new MailAddress(FromEmail);
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		try
		{
			await SmtpClient.SendMailAsync(message);
		}
		catch (Exception) { }
	}
}
