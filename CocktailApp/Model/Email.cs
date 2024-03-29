using System.Net.Mail;
using System.Net;

namespace CocktailApp.Model
{
  public class Email
    {
      public static bool Send(string email, string Subject, string body, bool isHtmlBody = false)
        {
          bool status = false;
          try
          {
            string HostAddress = "smtp-mail.outlook.com"; //for outlook emails
            string FormEmailId = "yourEmail";
            string Password = "YourPassword";
            string Port = "587"; //smtp port
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(FormEmailId);
            mailMessage.Subject = Subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = isHtmlBody;
            mailMessage.To.Add(new MailAddress(email));
            SmtpClient smtp = new SmtpClient();
            smtp.Host = HostAddress;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            NetworkCredential networkCredential = new NetworkCredential();
            networkCredential.UserName = mailMessage.From.Address;
            networkCredential.Password = Password;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = networkCredential;
            smtp.Port = Convert.ToInt32(Port);
            smtp.Send(mailMessage);
            status = true;
        }
          catch (Exception ex)
          {
            Console.Error.WriteLine(ex.Message);
            status = false;
          }
            return status;
        }
    }
}