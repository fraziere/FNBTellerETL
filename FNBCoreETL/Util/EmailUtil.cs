using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Util
{
    public static class EmailUtil
    {
        //private static int stopRecursion = 0;
        //private static readonly string appName = "FNB Teller ETL Utility";
        //private static readonly string smtpAddr = Configuration.Email.SmtpAddress.Value;
        //private static readonly string emailAlertFrom = Configuration.Email.SendProgramEmailAlertFrom.Value;
        //private static readonly string env = Configuration.Environment.Value;
        //private static List<string> emailAlertTo = Configuration.Email.SendProgramEmailAlertTo;



        //public static void SendAlert(string msg)
        //{
        //    if (Configuration.Email.SendProgramEmailAlert.Value)
        //    {
        //        try
        //        {
        //            using (MailMessage mailMsg = new MailMessage())
        //            {

        //                mailMsg.Subject = String.Format("{0}: {1}",
        //                    env,
        //                    appName);

        //                mailMsg.From = new MailAddress(emailAlertFrom);

        //                foreach (string to in emailAlertTo)
        //                {
        //                    mailMsg.To.Add(to);
        //                }

        //                mailMsg.Body = msg;

        //                using (SmtpClient client = new SmtpClient())
        //                {
        //                    client.Host = smtpAddr;
        //                    //client.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPswd);
        //                    client.Send(mailMsg);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            if (stopRecursion == 0)
        //            {
        //                stopRecursion++;
        //                //Logger.LogError(Logger.LogSubType.Email, "EmailUtil.TrySendProgramErrorsEmail", ex);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Sends email with the given parameters. Performs null checking on parameters.
        /// Note that attachment should be the filepath to the file to be attached.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachment">if included should be a filepath</param>
        public static void SendEmail(string from, string[] to, string subject = null, string body = null, string attachment = null)
        {
            try
            {
                using (MailMessage mailMsg = new MailMessage())
                {
                    mailMsg.From = new MailAddress(from);
                    foreach (var toAddress in to)
                    {
                        mailMsg.To.Add(toAddress);
                    }

                    if (subject != null)
                    {
                        mailMsg.Subject = subject;
                    }

                    if (body != null)
                    {
                        mailMsg.Body = body;
                    }

                    if (attachment != null)
                    {
                        mailMsg.Attachments.Add(new Attachment(attachment));
                    }

                    using (SmtpClient client = new SmtpClient())
                    {
                        client.Host = "smtpout.myfnb.us";
                        client.Send(mailMsg);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Same as other SendEmail method but this method supports multiple attachments.
        /// Sends email with the given parameters. Performs null checking on parameters.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachments">if included should be filepaths</param>
        public static void SendEmail(string from, string[] to, string subject = null, string body = null, string[] attachments = null)
        {
            try
            {
                using (MailMessage mailMsg = new MailMessage())
                {
                    mailMsg.From = new MailAddress(from);
                    foreach (var toAddress in to)
                    {
                        mailMsg.To.Add(toAddress);
                    }

                    if (subject != null)
                    {
                        mailMsg.Subject = subject;
                    }

                    if (body != null)
                    {
                        mailMsg.Body = body;
                    }

                    if (attachments != null)
                    {
                        foreach (var attachment in attachments)
                            mailMsg.Attachments.Add(new Attachment(attachment));
                    }

                    using (SmtpClient client = new SmtpClient())
                    {
                        client.Host = "smtpout.myfnb.us";
                        client.Send(mailMsg);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
