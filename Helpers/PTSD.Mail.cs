using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Net.Mail;

namespace PTSDProject.Helpers
{
    /// <summary>
    /// PTSD 郵件服務
    /// 提供郵件發送功能，支援附件、CC、BCC
    /// 移植自 Bia 專案
    /// </summary>
    public class Mail
    {
        /// <summary>
        /// 發送簡單郵件
        /// </summary>
        /// <param name="MailFrom">寄件者 Email</param>
        /// <param name="MailTo">收件者 Email (多個用逗號或分號分隔)</param>
        /// <param name="MailCC">副本 Email (多個用逗號或分號分隔)</param>
        /// <param name="MailBcc">密件副本 Email (多個用逗號或分號分隔)</param>
        /// <param name="Subject">郵件主旨</param>
        /// <param name="IsBodyHtml">內容是否為 HTML 格式</param>
        /// <param name="Body">郵件內容</param>
        /// <param name="Attachments">附件路徑 (多個用逗號分隔)</param>
        public static void SendSimpleMail(
            string MailFrom, 
            string MailTo, 
            string MailCC, 
            string MailBcc, 
            string Subject, 
            bool IsBodyHtml, 
            string Body, 
            string Attachments = "")
        {
            // 載入設定
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            string _smtpServer = config.GetValue<string>("AppSettings:GLOBAL_SMTPServer") ?? "smtp.gmail.com";
            string _mailAccount = config.GetValue<string>("AppSettings:MailAccount") ?? "";
            string _mailPassword = config.GetValue<string>("AppSettings:MailPwd") ?? "";
            string _appTitle = config.GetValue<string>("AppSettings:AppTitle") ?? "PTSD Project";

            // 驗證必要參數
            if (string.IsNullOrEmpty(MailFrom) || string.IsNullOrEmpty(MailTo))
            {
                // 不 throw exception，避免影響主流程
                Console.WriteLine("⚠️ Mail: 寄件者或收件者為空白");
                return;
            }

            if (string.IsNullOrEmpty(_mailAccount) || string.IsNullOrEmpty(_mailPassword))
            {
                Console.WriteLine("⚠️ Mail: SMTP 帳號或密碼未設定");
                return;
            }

            // 建立郵件物件
            MailMessage objMail = new MailMessage();
            objMail.From = new MailAddress(MailFrom, _appTitle);

            // 處理收件者 (支援逗號和分號分隔)
            MailTo = MailTo.Replace(";", ",");
            string[] mt = MailTo.Split(',');
            foreach (string s in mt)
            {
                if (!string.IsNullOrWhiteSpace(s) && s.Contains('@') && s.Contains('.'))
                {
                    try
                    {
                        objMail.To.Add(s.Trim());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Mail: 無效的收件者 Email: {s} - {ex.Message}");
                    }
                }
            }

            // 處理副本 (CC)
            if (!string.IsNullOrEmpty(MailCC))
            {
                MailCC = MailCC.Replace(";", ",");
                string[] mcc = MailCC.Split(',');
                foreach (string s in mcc)
                {
                    if (!string.IsNullOrWhiteSpace(s) && s.Contains('@') && s.Contains('.'))
                    {
                        try
                        {
                            objMail.CC.Add(s.Trim());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️ Mail: 無效的 CC Email: {s} - {ex.Message}");
                        }
                    }
                }
            }

            // 處理密件副本 (BCC)
            if (!string.IsNullOrEmpty(MailBcc))
            {
                MailBcc = MailBcc.Replace(";", ",");
                string[] mbcc = MailBcc.Split(',');
                foreach (string s in mbcc)
                {
                    if (!string.IsNullOrWhiteSpace(s) && s.Contains('@') && s.Contains('.'))
                    {
                        try
                        {
                            objMail.Bcc.Add(s.Trim());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️ Mail: 無效的 BCC Email: {s} - {ex.Message}");
                        }
                    }
                }
            }

            // 設定郵件格式
            objMail.IsBodyHtml = IsBodyHtml;

            // 格式化郵件內容 (加入標準頁首和頁尾)
            Body = $"<div style='font-family:微軟正黑體'>Dear Sir/Madam, <br />{Body}";
            Body += $@"</div><span style='font-family: Arial, Helvetica, sans-serif'>
                        <br />This email is sent from {_appTitle} service<br />
                        Please do not reply this mail directly.</span>";

            objMail.Subject = Subject;
            objMail.Body = Body;

            // 處理附件
            if (!string.IsNullOrEmpty(Attachments))
            {
                foreach (string filePath in Attachments.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                    {
                        try
                        {
                            Attachment data = new Attachment(filePath, System.Net.Mime.MediaTypeNames.Application.Octet);
                            System.Net.Mime.ContentDisposition disposition = data.ContentDisposition;
                            disposition.CreationDate = File.GetCreationTime(filePath);
                            disposition.ModificationDate = File.GetLastWriteTime(filePath);
                            disposition.ReadDate = File.GetLastAccessTime(filePath);
                            objMail.Attachments.Add(data);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️ Mail: 無法附加檔案: {filePath} - {ex.Message}");
                        }
                    }
                }
            }

            // 建立 SMTP Client
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = _smtpServer;

            // 非同步發送完成事件處理
            smtpClient.SendCompleted += (s, e) =>
            {
                SendCompletedCallback(s, e);
                smtpClient.Dispose();
                objMail.Dispose();
            };

            // 設定認證
            smtpClient.Credentials = new System.Net.NetworkCredential(_mailAccount, _mailPassword);

            // 非同步發送郵件
            try
            {
                smtpClient.SendAsync(objMail, "mail_token");
                Console.WriteLine($"📧 Mail: 正在發送郵件給 {MailTo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Mail: 發送失敗 - {ex.Message}");
                smtpClient.Dispose();
                objMail.Dispose();
            }
        }

        /// <summary>
        /// 郵件發送完成回調
        /// </summary>
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // 取得非同步操作的識別碼
            string token = (string)e.UserState;
            string strReturn = string.Empty;

            if (e.Cancelled)
            {
                strReturn = "Send canceled.";
                Console.WriteLine($"⚠️ Mail: {strReturn}");
            }
            else if (e.Error != null)
            {
                strReturn = "Send error: " + e.Error.ToString();
                Console.WriteLine($"❌ Mail: {strReturn}");
            }
            else
            {
                strReturn = "Send ok.";
                Console.WriteLine($"✅ Mail: {strReturn}");
            }
        }
    }
}

