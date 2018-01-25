using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace TigerLineScores.Models
{
    public class MessageInfo
    {

        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        public void SendEmail(string Recipient, string Subject, string Body, string attachment)
        {

            SmtpClient client = new SmtpClient();

            var message = new MailMessage();
            message.To.Add(new MailAddress(Recipient));
            message.Subject = Subject;
            message.IsBodyHtml = true;
            message.Body = Body;

            if (attachment != null)
            {
                message.Attachments.Add(new Attachment(attachment));
            }

            client.Send(message);            
        }



        public void CreateMessage(int SentID, int ReceivedID, DateTime DateSent, string MessageText)
        {
            Message newMessage = new Message();
            newMessage.SentByID = SentID;
            newMessage.ReceivedByID = ReceivedID;
            newMessage.DateSent = DateSent;
            newMessage.MessageText = MessageText;
            newMessage.MessageRead = false;
            db.Messages.Add(newMessage);
            db.SaveChanges();
        }


    }
}