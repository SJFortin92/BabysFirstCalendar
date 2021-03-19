using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using BabysFirstCalendar.DatabaseModels;
using static BabysFirstCalendar.EmailManagement.EmailLogic;
using static BabysFirstCalendar.DatabaseBusinessLogic.RetrievalProcessor;
using System.Net;

//Please note that SMTP emails are not recommended for security purposes
//Do not use this method in live production

namespace BabysFirstCalendar.EmailManagement
{
    public static class SMTPManager
    {
        //Function to compose the body of an email
        public static string ComposeBody(string firstName, DateTime dateLastUsed)
        {
            //Probably should insert links to go straight to "make a new memory" and "edit account"
            string body = "Hello " + firstName + ",<br/>";
            body += "This is your reminder to put in a new memory in your Baby's First Calendar. <br/>";
            body += "The last recorded memory was on " + dateLastUsed + ".<br/>";
            body += "If you want to turn off or change reminders, please go to your account and change your notification schedule.<br>";
            body += "<br/><br/>Please do not reply to this account. This account is not monitored.";
            return body;
        }

        //Sends the message to the user
        
        public static int SendMessage(string receiverEmail, string firstName, DateTime dateLastUsed)
        {
            //Set all our variables for the email we need
            string sendingEmail = "email";
            string fromName = "Baby's First Calendar";
            string password = "Password";
            string messageSubject = "Reminder to make a new memory";
            string body = ComposeBody(firstName, dateLastUsed);

            //Make a new MailAddress item for the sender and receiver
            MailAddress sendingInfo = new MailAddress(sendingEmail, fromName, System.Text.Encoding.UTF8);
            MailAddress receivingInfo = new MailAddress(receiverEmail, firstName, System.Text.Encoding.UTF8);
            
            //Try to send our email
            try 
            {
                //Set up our smtpClient
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential(sendingEmail, password),
                    EnableSsl = true,
                };

                //Compose our message
                using (MailMessage message = new MailMessage(sendingInfo, receivingInfo)
                {
                    Subject = messageSubject,
                    Body = body,
                    IsBodyHtml = true,
                    BodyEncoding = System.Text.Encoding.UTF8,
                })
                //Send our message
                {
                    smtpClient.Send(message);
                }
                return 1;
            }
            //If it fails, catch it
            catch
            {
                return 0;
            }
        }

        //Loops through each of the accounts and sends an email if 
        //the account needs a reminder
        public static int IterateAccounts()
        {
            List<AccountRemindersDBModel> listOfAccounts = PullNotificationAccounts();

            //For each account in the list of accounts...
            foreach (var account in listOfAccounts)
            {
                if (NeedsReminder(account))
                {
                    int success = SendMessage(account.Email, account.FirstName, account.DateLastUsed);
                    return success;
                }
            }

            //If we went through all the accounts and no one needed an email, or if all the emails were successful
            return 1;
        }
    }
}