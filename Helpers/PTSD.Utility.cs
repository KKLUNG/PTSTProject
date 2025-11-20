using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PTSDProject
{
    /// <summary>
    /// PTSD 專案工具類別
    /// 提供加密解密、資料轉換、HTML 處理、工作流列舉、金額轉文字等功能
    /// 移植自 Bia 專案，排除 PDF 浮水印功能
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// 加密金鑰 (32 bytes = 256 bits)
        /// </summary>
        private static readonly string EncryptKey = "466A9A1C2D054805BF3AE7E5C45B838B66F1418AAC3E4340B3E7F4DAD361FE2C";

        #region 權限列舉

        /// <summary>
        /// 權限識別碼 (ACL ID)
        /// </summary>
        public enum ACLID
        {
            NONE = 0,       // 無權限
            SHOW = 1,       // 顯示
            READ = 2,       // 瀏覽
            EXPORT = 4,     // 匯出
            INSERT = 8,     // 新增
            UPDATE = 16,    // 修改
            DELETE = 32     // 刪除
        }

        #endregion

        #region 工作流列舉

        /// <summary>
        /// 工作流狀態
        /// </summary>
        public enum FlowStatus
        {
            A_ActionStart = 0,                          // 待處理
            H_Handled_Approve = 1,                      // 送件
            C_Handled_Reject = 2,                       // 退件
            Z_NotHandle_UserWithDrawn = 3,              // 申請者撤回
            Q_Handled_Reject_ButWaitOthers = 4,         // 退件-等待他人中
            G_Handled_ButPlusSign = 5,                  // 關卡簽核者送出加簽(新增一筆G)
            L_Handled_ButPlusSign = 21,                 // 關卡簽核者送出加簽(原本的A->L)
            D_PlusSign = 14,                            // 被加簽
            F_PlusSignFinished = 15,                    // 被加簽完成
            W_Handled_Approve_ButWaitOthers = 6,        // 送件-等待他人中
            X_NotHandle_OthersAlreadySign = 7,          // 未簽不候
            B_Notify = 8,                               // 知會
            E_NotifyAfter_B = 9,                        // 知會確認
            N_Notify = 10,                              // 完成通知
            T_NotifyAfter_N = 11,                       // 通知確認
            R_Notify = 12,                              // 退件通知
            V_NotifyAfter_R = 13,                       // 退件確認
            Y_IsEnd = 16,                               // 結束
            I_Transfer_Approve = 17,                    // 核轉
            P_Handled_Pass = 18,                        // Pass => 該關卡無人, 移轉下一關
            J_Handled_Approve = 19,                     // 核決(該關卡若為核決關卡, 改壓J)
            K_Handled_Approve_NoOpinion = 20,           // 送件(流程中承辦只填寫表單內容, 無需發表意見)
            M_Handled_Approve_CounterSign = 21,         // 送件(會畢)
            U_Handled_Reject_ToApplierModify_Approver = 22,     // 退申請人修改(簽核者)
            O_Handled_Reject_ToApplierModify_Applier = 23,      // 退申請人修改(申請者)
            S_Handled_Reject_SendToCurrentStepApprover = 24,    // 退申請人修改(修改完成後送回原簽核者)
            Code_1_NotHandle_OthersAlreadySign = 25,    // 他人已審
            Code_2_Handled_RejectToPrevious = 26        // 退前一關
        }

        /// <summary>
        /// 工作流成員類型
        /// </summary>
        public enum FlowMemberType
        {
            User = 1,                               // 使用者
            DeptAdmin = 2,                          // 某部門主管
            Rela = 3,                               // 上一關卡的關係人
            Applier_rela = 4,                       // 申請者的關係人
            SystemFieldForUser = 5,                 // 系統欄位變數
            Group = 6,                              // 群組成員
            DeptMember = 7,                         // 某部門成員
            SystemFieldForDeptManager = 10,         // 系統欄位變數 for DeptManager
            SystemFieldForDeptMember = 11,          // 系統欄位變數 for DeptMember
            SystemFieldForGroupsMember = 12,        // 系統欄位變數 for Groups
            SystemFieldForDeptManager_Level1 = 13,  // 系統欄位變數 for 部門主管關係人第一階
            SystemFieldForDeptManager_Level2 = 14,  // 系統欄位變數 for 部門主管關係人第二階
            SystemFieldForDeptManager_Level3 = 15   // 系統欄位變數 for 部門主管關係人第三階
        }

        /// <summary>
        /// 工作流步驟類型
        /// </summary>
        public enum FlowStepType
        {
            Serial = 1,         // 串簽
            StartSubFlow = 2,   // 開始子流程
            Parallel = 4,       // 並簽
            DependOnFirst = 5,  // 依首簽
            EndSubFlow = 6,     // 結束子流程
            Notify = 7,         // 核定通知
            Inform = 8,         // 知會
            SubFlow = 9         // 子流程(單一人串簽)
        }

        /// <summary>
        /// 推播方式
        /// </summary>
        public enum PushBy
        {
            APP = 1,
            SKYPE = 2
        }

        #endregion

        #region JSON/DataTable 欄位檢查

        /// <summary>
        /// 檢查 JSON 物件是否包含指定的 Key
        /// </summary>
        public static bool CheckObjectKey(System.Text.Json.JsonElement obj, string keyValue)
        {
            try
            {
                JObject o = JObject.Parse(obj.ToString());
                if (o.ContainsKey(keyValue))
                {
                    return o.GetValue(keyValue) != null;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 檢查 DataTable 是否包含指定的欄位
        /// </summary>
        public static bool CheckObjectKey(DataTable dt, string keyValue)
        {
            try
            {
                DataColumnCollection columns = dt.Columns;
                return columns.Contains(keyValue);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 檢查 Dictionary 是否包含指定的 Key
        /// </summary>
        public static bool CheckObjectKey(Dictionary<string, string> obj, string keyValue)
        {
            return obj.TryGetValue(keyValue, out _);
        }

        /// <summary>
        /// 檢查 DataTable 是否包含指定欄位名稱
        /// </summary>
        public static bool CheckColumnExist(string columnName, DataTable dt)
        {
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName == columnName)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 資料轉換 (DataTable → SQL)

        /// <summary>
        /// 將 DataTable 轉換為 INSERT SQL 語句
        /// </summary>
        /// <param name="TableName">目標資料表名稱</param>
        /// <param name="dt">來源 DataTable</param>
        /// <returns>INSERT SQL 語句</returns>
        public static string GetSQL(string TableName, DataTable dt)
        {
            string ssql = string.Empty;
            string columnName = string.Empty;

            // 組合欄位名稱
            foreach (DataColumn dc in dt.Columns)
            {
                columnName += dc.ColumnName + ", ";
            }
            columnName = columnName.Substring(0, columnName.Length - 2);
            columnName = "(" + columnName + ")";

            // 組合每一列的資料
            foreach (DataRow dr in dt.Rows)
            {
                string columnValue = string.Empty;
                foreach (DataColumn dc in dt.Columns)
                {
                    // 處理日期時間欄位
                    if (dc.ColumnName == "CreatedDate" || dc.ColumnName == "ModifiedDate")
                    {
                        if (dr[dc.ColumnName].ToString() == string.Empty)
                        {
                            columnValue += "null, ";
                        }
                        else
                        {
                            DateTime d = DateTime.Parse(dr[dc.ColumnName].ToString());
                            columnValue += "'" + d.ToString("s").Replace("T", " ") + "', ";
                        }
                    }
                    // 處理使用者 ID 欄位
                    else if (dc.ColumnName == "CreatedUserID" || dc.ColumnName == "ModifiedUserID")
                    {
                        if (dr[dc.ColumnName].ToString() == string.Empty)
                        {
                            columnValue += "null, ";
                        }
                        else
                            columnValue += "'" + dr[dc.ColumnName].ToString() + "', ";
                    }
                    // 一般欄位
                    else
                    {
                        columnValue += "N'" + dr[dc.ColumnName].ToString().Replace("'", "''") + "', ";
                    }
                }
                columnValue = columnValue.Substring(0, columnValue.Length - 2);
                columnValue = "(" + columnValue + ")";

                ssql += $"INSERT INTO {TableName} {columnName} VALUES {columnValue};";
            }

            return ssql;
        }

        #endregion

        #region 多語系處理

        /// <summary>
        /// 取得多語系標題
        /// </summary>
        /// <param name="FCaption">預設標題</param>
        /// <param name="FCaptionLang">多語系 JSON</param>
        /// <param name="language">目標語言</param>
        /// <returns>對應語言的標題</returns>
        public static string GetCaption(string FCaption, string FCaptionLang, string language)
        {
            try
            {
                if (language == "zhTW")
                    return FCaption;
                else
                {
                    if (string.IsNullOrEmpty(FCaptionLang))
                        return FCaption;
                    else
                    {
                        var j = JObject.Parse(FCaptionLang);
                        if (j[language] == null || j[language].ToString() == string.Empty)
                            return FCaption;
                        else
                            return j[language].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                string aa = ex.Message;
                return FCaption;
            }
        }

        #endregion

        #region HTML 處理

        /// <summary>
        /// 去除 HTML 標籤，可自訂合法標籤加以保留
        /// </summary>
        /// <param name="src">來源字串</param>
        /// <param name="reservedTagPool">合法標籤集</param>
        /// <returns>處理後的字串</returns>
        public static string StripTags(string src, string[] reservedTagPool)
        {
            return Regex.Replace(
                src,
                String.Format("<(?!{0}).*?>", string.Join("|", reservedTagPool)),
                String.Empty);
        }

        /// <summary>
        /// 將 HTML 轉換為純文字 (移除常見的 HTML 標籤)
        /// </summary>
        public static string ReplaceHtml(string str)
        {
            if (str.Contains("<p>", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Replace("<p>", string.Empty, StringComparison.OrdinalIgnoreCase);
            }
            if (str.Contains("</p>", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Replace("</p>", System.Environment.NewLine, StringComparison.OrdinalIgnoreCase);
            }
            if (str.Contains("<br>", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Replace("<br>", System.Environment.NewLine, StringComparison.OrdinalIgnoreCase);
            }
            if (str.Contains("<br/>", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Replace("<br/>", System.Environment.NewLine, StringComparison.OrdinalIgnoreCase);
            }
            if (str.Contains("<br />", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Replace("<br />", System.Environment.NewLine, StringComparison.OrdinalIgnoreCase);
            }
            str = System.Web.HttpUtility.HtmlDecode(str);
            str = Regex.Replace(str, "<.*?>", string.Empty);

            return str;
        }

        /// <summary>
        /// 跳脫 SQL LIKE 查詢的特殊字元
        /// </summary>
        public static string EscapeValue(string value)
        {
            System.Text.StringBuilder sb = new();
            List<char> lc = value.ToList<char>();
            lc.ForEach(delegate (char ch)
            {
                if (ch == '*' || ch == '%' || ch == '[' || ch == ']')
                    sb.Append("[").Append(ch).Append("]");
                else
                    sb.Append(ch);
            });
            return sb.ToString();
        }

        #endregion

        #region DataSet 處理

        /// <summary>
        /// 移除 DataSet 中所有日期時間欄位的時區資訊
        /// </summary>
        public static void RemoveTimezoneForDataSet(DataSet ds)
        {
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.DataType == typeof(DateTime))
                    {
                        dc.DateTimeMode = DataSetDateTime.Unspecified;
                    }
                }
            }
        }

        /// <summary>
        /// 數字格式化
        /// </summary>
        public static string NF(decimal n, string f)
        {
            return n.ToString(f);
        }

        #endregion

        #region JSON 驗證

        /// <summary>
        /// 驗證字串是否為有效的 JSON
        /// </summary>
        public static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 檔案處理

        /// <summary>
        /// 從 URL 下載並儲存檔案
        /// </summary>
        public static async Task SaveFile(string fileUrl, string pathToSave)
        {
            var httpClient = new HttpClient();
            var httpResult = await httpClient.GetAsync(fileUrl);
            using var resultStream = await httpResult.Content.ReadAsStreamAsync();
            using var fileStream = File.Create(pathToSave);
            resultStream.CopyTo(fileStream);
        }

        #endregion

        #region 金額轉英文文字

        /// <summary>
        /// 金額轉英文文字
        /// </summary>
        public class AmountToWords
        {
            private static String[] units = { "Zero", "One", "Two", "Three",
                "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven",
                "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
                "Seventeen", "Eighteen", "Nineteen" };
            
            private static String[] tens = { "", "", "Twenty", "Thirty", "Forty",
                "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            /// <summary>
            /// 將金額轉換為英文文字
            /// </summary>
            /// <param name="amount">金額</param>
            /// <returns>英文文字描述</returns>
            public static String GetWords(decimal amount)
            {
                try
                {
                    Int64 amount_int = (Int64)amount;
                    Int64 amount_dec = (Int64)Math.Round((amount - (decimal)(amount_int)) * 100);
                    
                    if (amount_dec == 0)
                    {
                        return Convert(amount_int) + " Only.";
                    }
                    else
                    {
                        return Convert(amount_int) + " And Cents " + Convert(amount_dec) + " Only.";
                    }
                }
                catch (Exception)
                {
                    return "";
                }
            }

            private static String Convert(Int64 i)
            {
                if (i < 20)
                {
                    return units[i];
                }
                if (i < 100)
                {
                    return tens[i / 10] + ((i % 10 > 0) ? " " + Convert(i % 10) : "");
                }
                if (i < 1000)
                {
                    return units[i / 100] + " Hundred" + ((i % 100 > 0) ? " " + Convert(i % 100) : "");
                }
                if (i < 100000)
                {
                    return Convert(i / 1000) + " Thousand " + ((i % 1000 > 0) ? " " + Convert(i % 1000) : "");
                }
                if (i < 10000000)
                {
                    return Convert(i / 100000) + " Hundred Thousand " + ((i % 100000 > 0) ? " " + Convert(i % 100000) : "");
                }
                if (i < 1000000000)
                {
                    return Convert(i / 10000000) + " Crore " + ((i % 10000000 > 0) ? " " + Convert(i % 10000000) : "");
                }
                return Convert(i / 1000000000) + " Arab " + ((i % 1000000000 > 0) ? " " + Convert(i % 1000000000) : "");
            }
        }

        #endregion

        #region 檔案加密/解密

        /// <summary>
        /// 加密檔案
        /// </summary>
        /// <param name="FileFullName">來源檔案完整路徑</param>
        /// <param name="EncryptFilePath">加密後存放路徑</param>
        public static void EncryptFile(string FileFullName, string EncryptFilePath)
        {
            using (Aes aes = Aes.Create())
            {
                string FileName = FileFullName.Split('\\')[^1];
                EncryptFilePath = EncryptFilePath + "\\" + FileName.Split('.')[0] + ".aes";
                aes.Key = StringToByteArray(EncryptKey);
                aes.IV = StringToByteArray(EncryptKey.Substring(5, 32));

                using (FileStream fsInput = new FileStream(FileFullName, FileMode.Open, FileAccess.Read))
                using (FileStream fsOutput = new FileStream(EncryptFilePath, FileMode.Create, FileAccess.Write))
                using (BinaryWriter writer = new BinaryWriter(fsOutput))
                {
                    // 儲存原始副檔名
                    string originalExtension = Path.GetExtension(FileFullName);
                    writer.Write(originalExtension);

                    // 加密檔案內容
                    using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (CryptoStream cs = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] buffer = new byte[1048576]; // 1 MB buffer
                        int bytesRead;

                        while ((bytesRead = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cs.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 解密檔案
        /// </summary>
        /// <param name="FileFullName">加密檔案完整路徑</param>
        /// <param name="DecryptFilePath">解密後存放路徑</param>
        public static void DecryptFile(string FileFullName, string DecryptFilePath)
        {
            using (Aes aes = Aes.Create())
            {
                string FileName = FileFullName.Split('\\')[^1];
                DecryptFilePath = DecryptFilePath + "\\" + FileName;

                aes.Key = StringToByteArray(EncryptKey);
                aes.IV = StringToByteArray(EncryptKey.Substring(5, 32));

                using (FileStream fsInput = new FileStream(FileFullName, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(fsInput))
                {
                    // 讀取原始副檔名
                    string originalExtension = reader.ReadString();

                    // 產生解密檔案路徑
                    string decryptedFilePath = Path.ChangeExtension(DecryptFilePath, originalExtension);

                    using (FileStream fsOutput = new FileStream(decryptedFilePath, FileMode.Create, FileAccess.Write))
                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (CryptoStream cs = new CryptoStream(fsInput, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] buffer = new byte[1048576]; // 1 MB buffer
                        int bytesRead;

                        while ((bytesRead = cs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fsOutput.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 將十六進位字串轉換為 Byte 陣列
        /// </summary>
        private static byte[] StringToByteArray(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        #endregion
    }
}

