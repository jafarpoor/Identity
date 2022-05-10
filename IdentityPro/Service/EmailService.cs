using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IdentityPro.Service
{
    public class EmailService
    {
        public Task Excute(string EmailUser , string Body , string Title)
        {
            //تنظیمات گوگل حتما باید انجام بشه
          //https://myaccount.google.com/lesssecureapps
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 1000000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            //در خط بعدی ایمیل  خود و پسورد ایمیل خود  را جایگزین کنید
            client.Credentials = new NetworkCredential("faezehjafarpour75@gmail.com", "Faezeh1375!");
            MailMessage message = new MailMessage();
            message.Subject = Title;
            message.Body = Body;
            message.From = new MailAddress("faezehjafarpour75@gmail.com");
            message.To.Add(EmailUser);
            message.IsBodyHtml = true;
            client.Send(message);
            return Task.CompletedTask;

        }
    }
}
