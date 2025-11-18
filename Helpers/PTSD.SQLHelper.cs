using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Xml;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Hosting.Server;

namespace PTSDProject
{
    public sealed class SqlHelper
    {
        //連線資料庫字串
        #region private utility methods & constructors
        private static readonly Lazy<IConfiguration> _config = new Lazy<IConfiguration>(() =>
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();
        });
        
        private static string _connString => _config.Value.GetConnectionString("PTSDContext") 
            ?? throw new InvalidOperationException("Connection string 'PTSDContext' not found in appsettings.json");

        // 若不想使用 "new SqlHelper()" 則將宣告為 private constructor
        private SqlHelper()
        {
            //增加這段

        }

        //在動態組 SQL 前置換使用者/品牌/語系/流程等情境參數，常見於 CMS 取資料或過濾條件的場景。
        //2025.10.02 先放著，也許以後用得到

        public static string SQLReplace(string ssql, string UserId, string UserGuid, string KeyParameter, string ParentParameter, string Lang, string XMLParameter, string Brand, string FactoryId, string wfSNMsgGuid, string IsMobile)
        {
            ssql = ssql.Replace("{UserId}", UserId, StringComparison.OrdinalIgnoreCase);
            ssql = ssql.Replace("{UserGuid}", UserGuid, StringComparison.OrdinalIgnoreCase);
            ssql = ssql.Replace("{KeyParameter}", KeyParameter, StringComparison.OrdinalIgnoreCase);
            ssql = ssql.Replace("{ParentParameter}", ParentParameter, StringComparison.OrdinalIgnoreCase);
            ssql = ssql.Replace("{Lang}", Lang, StringComparison.OrdinalIgnoreCase);
            ssql = ssql.Replace("{Brand}", Brand, StringComparison.OrdinalIgnoreCase);
            ssql = ssql.Replace("{FactoryId}", FactoryId, StringComparison.OrdinalIgnoreCase);
            ssql = ssql.Replace("{wfSNMsgGuid}", wfSNMsgGuid, StringComparison.OrdinalIgnoreCase);
            ssql = ssql.Replace("{IsMobile}", IsMobile, StringComparison.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(XMLParameter) && (ssql.Contains("{0}") || ssql.Contains("{1}") || ssql.Contains("{2}") || ssql.Contains("{3}")))
            {
                var a = XMLParameter.Split(',');
                for (var i = 0; i < a.Length; i++)
                {
                    a[i] = a[i].Replace("$$$", "/"); //這是因為cmsframe在拆解時會錯, 所以要先改成^^^  ex: CMSFRame/AdminGridView/3/Form_aaa.xml;abcde/aaaaa   這種XMLParameter會死掉.
                    a[i] = a[i].Replace("$$", ",");  //因為字串中含有', / ',會導致XMLParameter split 出錯

                    a[i] = a[i].Replace("^^^", "/"); //這是因為cmsframe在拆解時會錯, 所以要先改成^^^  ex: CMSFRame/AdminGridView/3/Form_aaa.xml;abcde/aaaaa   這種XMLParameter會死掉.
                    a[i] = a[i].Replace("^^", ",");  //因為字串中含有', / ',會導致XMLParameter split 出錯
                }
                ssql = string.Format(ssql, a);  //這裡會有json format 錯誤的問題 '[{xxxx}]
            }
            else
            {
                if (ssql.Contains("{0}"))
                    ssql = string.Format(ssql, "00000000-0000-0000-0000-000000000000"); //預防有 {0} 但卻沒有東西 , 不用string.Empty 是考慮有可能是GUID欄位
            }

            return ssql;
        }

        /// <summary>
        /// This method is used to attach array of SqlParameters to a SqlCommand.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of SqlParameters to be added to command</param>
        //private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        //{
        //    if (command == null) throw new ArgumentNullException("command");
        //    if (commandParameters != null)
        //    {
        //        foreach (SqlParameter p in commandParameters)
        //        {
        //            if (p != null)
        //            {
        //                // Check for derived output value with no value assigned
        //                if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && (p.Value == null))
        //                {
        //                    p.Value = DBNull.Value;
        //                }
        //                command.Parameters.Add(p);
        //            }
        //        }
        //    }
        //}

        #endregion private utility methods & constructors



        #region 預設連線，純文字 SQL
        //只有 SQL 字串，走預設連線
        //ExecuteDataset(string commandText)
        //已建好 SqlCommand（含參數/逾時），走預設連線
        //ExecuteDataset(SqlCommand cmd)
        //要指定不同資料庫/租戶（自訂連線字串）
        //ExecuteDataset(string connectionString, CommandType, string, [params])
        //ExecuteDataset(string connectionString, SqlCommand cmd)
        //已有開啟中的 SqlConnection 由你管理
        //ExecuteDataset(SqlConnection connection, ...)
        //正在一個交易 SqlTransaction 裡
        //ExecuteDataset(SqlTransaction transaction, ...)
        //呼叫 Stored Procedure 且不想手動組 SqlParameter[]
        //ExecuteDataset(<connString|SqlConnection>, string spName, params object[] parameterValues)

        //需要切 DB/租戶 → 傳入 connectionString。
        public static DataSet ExecuteDataset(string commandText)
        {
            return ExecuteDataset(_connString, CommandType.Text, commandText);
        }

        //預設連線，已建好 SqlCommand
        //你已設定好 SQL、參數、逾時等；用 _connString 執行。
        public static DataSet ExecuteDataset(SqlCommand cmd)
        {
            return ExecuteDataset(_connString, cmd);
        }
        #endregion


        #region 指定連線字串，已建好 SqlCommand
        //你已設定好 SQL、參數、逾時等；用 _connString 執行。
        //但可指定不同 DB/租戶。
        public static DataSet ExecuteDataset(string connectionString, SqlCommand cmd)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

            // Create & open a SqlConnection, and dispose of it after we are done
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                cmd.Connection = connection;
                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataset(connection, cmd);
            }
        }
        #endregion

        #region 指定連線字串，純文字 SQL（兩種）
        //第一:無參數。
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        //第二:有參數。
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

            // Create & open a SqlConnection, and dispose of it after we are done
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            // Call the overload that takes a connection in place of the connection string
            return ExecuteDataset(connection, commandType, commandText, commandParameters);
        }
        #endregion

        #region 指定連線字串，存放程序（SP）
        //自動從快取/DB推導 SP 參數，依序代入值。
        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region 已有開啟的 SqlConnection（外部管理連線生命週期）
        //直接以 SqlDataAdapter 填入 DataSet。
        public static DataSet ExecuteDataset(SqlConnection connection, SqlCommand cmd)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            //bool mustCloseConnection = false;

            // Create the DataAdapter & DataSet
            //string CertificateKey = config.GetValue<string>("AppSettings:CertificateKey");
            //string SymmetricKey = config.GetValue<string>("AppSettings:SymmetricKey");
            //cmd.CommandText = $"OPEN SYMMETRIC KEY {SymmetricKey} DECRYPTION BY CERTIFICATE [{CertificateKey}]; " + System.Environment.NewLine + cmd.CommandText + ";" + System.Environment.NewLine + " CLOSE ALL SYMMETRIC KEYS  ";
            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            using DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.Parameters.Clear();

            //if (mustCloseConnection)
            connection.Close();

            return ds;
        }

        //無參數。 
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null);
        }

        //有參數
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            using SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);
            //string CertificateKey = config.GetValue<string>("AppSettings:CertificateKey");
            //string SymmetricKey = config.GetValue<string>("AppSettings:SymmetricKey");
            //cmd.CommandText = $"OPEN SYMMETRIC KEY {SymmetricKey} DECRYPTION BY CERTIFICATE [{CertificateKey}]; " + System.Environment.NewLine + cmd.CommandText + ";" + System.Environment.NewLine + " CLOSE ALL SYMMETRIC KEYS  ";
            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            using DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
                cmd.Parameters.Clear();

                //if (mustCloseConnection)
                //    connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                throw new Exception(ex.Message);

            }
            finally
            {
                connection.Close();
            }
            return ds;

        }

        //SP，自動推導參數並代入。
        public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region 已有進行中的交易 SqlTransaction
        //無參數。
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
        }

        //SP，自動推導參數並代入。
        public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        //有參數。
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            using SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            //string CertificateKey = config.GetValue<string>("AppSettings:CertificateKey");
            //string SymmetricKey = config.GetValue<string>("AppSettings:SymmetricKey");
            //cmd.CommandText = $"OPEN SYMMETRIC KEY {SymmetricKey} DECRYPTION BY CERTIFICATE [{CertificateKey}]; " + System.Environment.NewLine + cmd.CommandText + ";" + System.Environment.NewLine + " CLOSE ALL SYMMETRIC KEYS  ";
            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            using DataSet ds = new DataSet();

            da.Fill(ds);

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();

            // Return the dataset
            return ds;
        }
        #endregion

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command
        /// </summary>
        /// <param name="command">The SqlCommand to be prepared</param>
        /// <param name="connection">A valid SqlConnection, on which to execute this command</param>
        /// <param name="transaction">A valid SqlTransaction, or 'null'</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
        // 說明：開啟連線（必要時）、指派連線/交易、命令類型與文字，並附加參數
        //AI回應
        //檢查輸入：command 與 commandText 不可為空。
        //開啟連線：若 connection 未開啟就 Open()，並設定 mustCloseConnection = true；否則為 false。
        //綁定命令：把 connection 指給 command，設定 command.CommandText。
        //套用交易：若有 transaction 且有效，指派到 command.Transaction。
        //設定型別：指定 command.CommandType（Text/StoredProcedure）。
        //加參數：若有 commandParameters，呼叫 AttachParameters 附加到 command。
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                try
                {
                    connection.Open();
                }
                catch (Exception e)
                {
                    string msg = "Database connection Error! Maybe your Database is not runing or database connection string is mistake?";
                    throw new Exception(msg, e);
                }
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        /// <summary>
        /// This method is used to attach array of SqlParameters to a SqlCommand.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of SqlParameters to be added to command</param>
        //功能：把 SqlParameter[] 逐一掛到 command.Parameters 上。
        //處理：對 Input/InputOutput 且值為 null 的參數改成 DBNull.Value，避免送 SQL Server 出錯。
        //注意：加入的是原參數物件引用；重複使用前要先 command.Parameters.Clear()，或為不同指令建立新的參數實例。
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }


        /// <summary>
        /// This method assigns an array of values to an array of SqlParameters
        /// </summary>
        /// <param name="commandParameters">Array of SqlParameters to be assigned values</param>
        /// <param name="parameterValues">Array of objects holding the values to be assigned</param>
        //功能：把一組值 parameterValues 依序指派到對應的 SqlParameter[] commandParameters。
        //檢查：任一為 null 直接返回；兩者長度不一致則丟出「參數個數不正確」。
        //指派規則：
        //若值是 IDbDataParameter，取其 Value；為 null 改成 DBNull.Value。
        //一般值為 null → 指派 DBNull.Value（資料庫的 NULL）。
        //其餘直接指派原值。
        //目的：在執行前正確填入參數，並用 DBNull.Value 表示資料庫可接受的空值。
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                // Do nothing if we get no data
                return;
            }

            // We must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("參數個數不正確");
            }

            // Iterate through the SqlParameters, assigning the values from the corresponding position in the 
            // value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                // If the current array value derives from IDbDataParameter, then assign its Value property
                if (parameterValues[i] is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                    if (paramInstance.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = paramInstance.Value;
                    }
                }
                else if (parameterValues[i] == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues[i];
                }
            }
        }

        //同功能
        //這段從 DataRow 依「名稱」配對參數：把每個 SqlParameter.ParameterName 去掉前導字元（如 @），用同名的資料行值填入參數。
        //與前一段（用 object[] parameterValues 依「位置」指派）不同，這裡是「依欄位名」指派。
        //會檢查參數名稱有效；若 DataRow 沒有對應欄位，就不變更該參數的值（不會自動設 DBNull.Value）。
        private static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || (dataRow == null))
            {
                // Do nothing if we get no data
                return;
            }

            int i = 0;
            // Set the parameters values
            foreach (SqlParameter commandParameter in commandParameters)
            {
                // Check the parameter name
                if (commandParameter.ParameterName == null || commandParameter.ParameterName.Length <= 1)
                    throw new Exception(string.Format("請提供參數名稱 parameter #{0}, 參數名稱請依循格式 '{1}'.", i, commandParameter.ParameterName));

                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                i++;
            }
        }


    }

    public sealed class SqlHelperParameterCache
    {
        #region private methods, variables, and constructors

        //Since this class provides only static methods, make the default constructor private to prevent 
        //instances from being created with "new SqlHelperParameterCache()"
        private SqlHelperParameterCache() { }

        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Resolve at run time the appropriate set of SqlParameters for a stored procedure
        /// </summary>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
        /// <returns>The parameter array discovered.</returns>
        private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            using SqlCommand cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            SqlCommandBuilder.DeriveParameters(cmd);
            connection.Close();

            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }

            SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];

            cmd.Parameters.CopyTo(discoveredParameters, 0);

            // Init the parameters with a DBNull value
            foreach (SqlParameter discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }

        /// <summary>
        /// Deep copy of cached SqlParameter array
        /// </summary>
        /// <param name="originalParameters"></param>
        /// <returns></returns>
        private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion private methods, variables, and constructors

        #region caching functions

        /// <summary>
        /// Add parameter array to the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters to be cached</param>
        public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        /// <summary>
        /// Retrieve a parameter array from the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An array of SqlParamters</returns>
        public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            SqlParameter[] cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        #endregion caching functions

        #region Parameter Discovery Functions

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of SqlParameters</returns>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of SqlParameters</returns>
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, false);
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            using (SqlConnection clonedConnection = (SqlConnection)((ICloneable)connection).Clone())
            {
                return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            SqlParameter[] cachedParameters;

            cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                SqlParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }

            return CloneParameters(cachedParameters);
        }

        #endregion Parameter Discovery Functions
    }
}
