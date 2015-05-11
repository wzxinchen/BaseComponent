using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.Utils
{
    public class EmailHelper
    {
        public readonly static string From = ConfigHelper.GetConfigAppSettingValue("EmailFrom");
        public readonly static string Username = ConfigHelper.GetConfigAppSettingValue("EmailUsername");
        public readonly static string Password = ConfigHelper.GetConfigAppSettingValue("EmailPassword");
        public readonly static string Smtp = ConfigHelper.GetConfigAppSettingValue("SMTP");
        public readonly static int Port = ConvertHelper.ToInt32(ConfigHelper.GetConfigAppSettingValue("EmailPort"));
        static EmailHelper()
        {
            if (string.IsNullOrWhiteSpace(From))
            {
                throw new Exception("邮件配置错误：AppSettings中的EmailFrom节点未配置");
            }
            if (string.IsNullOrWhiteSpace(Username))
            {
                throw new Exception("邮件配置错误：AppSettings中的EmailUsername节点未配置");
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new Exception("邮件配置错误：AppSettings中的EmailPassword节点未配置");
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new Exception("邮件配置错误：AppSettings中的SMTP节点未配置");
            }
            if (Port <= 0)
            {
                throw new Exception("邮件配置错误：AppSettings中的Port节点配置错误");
            }
        }
        public static void Send(string to, string title, string content)
        {
            MailMessage mailmessage = new MailMessage(From, to, title, content);
            //from email，to email，主题，邮件内容
            mailmessage.IsBodyHtml = true;
            mailmessage.Priority = MailPriority.Normal; //邮件优先级
            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(Smtp, Port); //smtp地址以及端口号
            smtpClient.Credentials = new NetworkCredential(Username, Password);//smtp用户名密码
            smtpClient.Send(mailmessage); //发送邮件
            smtpClient.Dispose();
            mailmessage.Dispose();
        }
    }
}
