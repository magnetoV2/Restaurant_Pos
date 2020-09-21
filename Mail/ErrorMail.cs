using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Restaurant_Pos.Mail
{
    class ErrorMail
    {
        public static void InvoicePostingLog()
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("a2plcpnl0221.prod.iad2.secureserver.net");

                mail.From = new MailAddress("lovin@zearoconsulting.com");
                mail.To.Add("lovin@zearoconsulting.com");
                mail.Subject = "Test Mail";
                mail.Body = "This is for testing SMTP mail from GMAIL";

                SmtpServer.Port = 465;
                SmtpServer.Credentials = new System.Net.NetworkCredential("lovin@zearoconsulting.com", "reset@123");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                Console.WriteLine("mail Send");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
