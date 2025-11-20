using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using PTSDProject.Models;

namespace PTSDProject.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class CMSController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private const string _EmptyGuid = "00000000-0000-0000-0000-000000000000";

        public CMSController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region 登入時需要的核心 API

        /// <summary>
        /// 獲取系統配置
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetCMSConfig()
        {
            try
            {
                using DataSet ds = SqlHelper.ExecuteDataset("SELECT * FROM CMSConfig WHERE isActive = '1'");
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);
                
                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 獲取多語言字典
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetCMSLang(string LangType)
        {
            try
            {
                using DataSet ds = SqlHelper.ExecuteDataset($"SELECT * FROM CMSLang WHERE LangType='{LangType}'");
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 獲取匯率資料
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetExchange()
        {
            try
            {
                using DataSet ds = SqlHelper.ExecuteDataset("SELECT CONVERT(decimal(18,6), CodesDesc1) AS ExchangeRate, CodesID as Currency FROM RS_CMSCodes('AL') ORDER BY CodesDesc3");
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);
                
                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 預載 XML 資料
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetCMSFormPreloadByXMLNames(string XMLNames)
        {
            try
            {
                if (string.IsNullOrEmpty(XMLNames))
                    return Ok("{}");

                // TODO: 實作預載邏輯
                // 目前返回空物件
                return Ok("{}");
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        #endregion

        #region 菜單相關 API

        /// <summary>
        /// 獲取主菜單（用於主菜單磁磚顯示）
        /// Add by Allen, use for Main tile query. 2023.11.13
        /// </summary>
        [HttpGet]
        public ActionResult GetCMSMainMenu(string UserGuid, string Language)
        {
            try
            {
                string ssql = @$"
                    -- Home order is 0
                    -- Homeguid = B.MenuGuid MenuGuid, RootGuid = A.MenuGuid MenuPGuid
                    SELECT ISNULL(dbo.GetLang(A.MenuLang,'{Language}'), A.MenuTitle) MenuTitle, 
                    (SELECT TOP 1 MenuGuid FROM CMSMenu WHERE MenuPGuid=A.MenuGuid ORDER BY MenuOrder) MenuGuid, 
                    A.MenuGuid MenuPGuid,
                    CASE WHEN ISNULL(A.MenuIcon,'')= '' THEN 'icon dx-icon-folder'
                    WHEN ISNULL(A.MenuIcon,'')LIKE '%icon-%' THEN A.MenuIcon
                    ELSE 'icon dx-icon-' + A.MenuIcon END MenuIcon
                    FROM CMSMenu A
                    LEFT JOIN CMSDisableMenu C ON A.MenuGuid = C.MenuGuid
                    INNER JOIN RS_ACL_CMSMenu('{UserGuid}') D ON A.MenuGuid = D.MenuGuid 
                    WHERE A.MenuPGuid = 'F18DEB5F-5205-4434-81AD-36E2E51AB835' AND D.ACLID > 0 AND C.MenuGuid IS NULL 
                    ORDER BY A.MenuOrder";

                using DataSet ds = SqlHelper.ExecuteDataset(ssql);
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 獲取用戶菜單樹（用於側邊欄菜單）
        /// </summary>
        [HttpGet]
        public ActionResult GetCMSMenuByUserId(string KeyParameter, string UserGuid, string Language, string IgnoreACL = "0", string DisplayMode = "1")
        {
            try
            {
                string ssql = "SELECT A.MenuGuid, A.MenuPGuid, A.HelpRemark, A.HelpFiles, A.HelpVideo, A.ACLSuccess, A.MenuSelfUrl, A.MenuTitle, A.MenuLang, A.MenuIcon, " + 
                             (IgnoreACL == "0" ? "C.ACLID" : "99") + @" AS ACLID , ISNULL(D.MenusCount,0) AS MenusCount 
                            FROM BAT_CMSMenu A INNER JOIN
                            (
	                            SELECT MenuGuid FROM BAT_CMSMenu WHERE MenuTitleAll LIKE '%' + (SELECT MenuTitleAll FROM BAT_CMSMenu 
	                            WHERE MenuGuid= '" + KeyParameter + @"' ) + '%'
	                            ) AS B ON A.MenuGuid = B.MenuGuid
                            INNER JOIN RS_ACL_CMSMenu('" + UserGuid + @"') AS C ON A.MenuGuid = C.MenuGuid
                            LEFT JOIN (SELECT COUNT(*) AS MenusCount, MenuGuid FROM CMSMenus GROUP BY MenuGuid) AS D ON A.MenuGuid = D.MenuGuid ";

                if (DisplayMode == "1")
                    ssql += " WHERE A.DisplayMode IN ('0','1') ";
                else
                    ssql += " WHERE A.DisplayMode IN ('0','2') ";

                if (IgnoreACL == "0")
                    ssql += "  AND C.ACLID > 0 ";

                ssql += " ORDER BY MenuOrder, MenuTitleAll ";

                using (DataSet ds = SqlHelper.ExecuteDataset(ssql))
                {
                    Dictionary<Guid, MenuItem> dict =
                       ds.Tables[0].Rows.Cast<DataRow>()
                                 .Select(r => new MenuItem
                                 {
                                     MenuGuid = r.Field<Guid>("MenuGuid"),
                                     MenuPGuid = r.Field<Guid>("MenuPGuid"),
                                     MenuTitle = GetCaption(r.Field<string>("MenuTitle"), r.Field<string>("MenuLang"), Language),
                                     Path = (r.Field<int>("MenusCount") == 0) ? string.Empty : "/CMSPage/" + r.Field<Guid>("MenuGuid"),
                                     MenuSelfUrl = (string.IsNullOrEmpty(r.Field<string>("MenuSelfUrl"))) ? string.Empty : r.Field<string>("MenuSelfUrl"),
                                     icon = (string.IsNullOrEmpty(r.Field<string>("MenuIcon"))) ? "folder" : r.Field<string>("MenuIcon"),
                                     HelpRemark = (string.IsNullOrEmpty(r.Field<string>("HelpRemark"))) ? string.Empty : r.Field<string>("HelpRemark"),
                                     HelpFiles = (string.IsNullOrEmpty(r.Field<string>("HelpFiles"))) ? string.Empty : r.Field<string>("HelpFiles"),
                                     HelpVideo = (string.IsNullOrEmpty(r.Field<string>("HelpVideo"))) ? string.Empty : r.Field<string>("HelpVideo"),
                                     ACLSuccess = (string.IsNullOrEmpty(r.Field<string>("ACLSuccess"))) ? string.Empty : r.Field<string>("ACLSuccess")
                                 })
                                .ToDictionary(m => m.MenuGuid);

                    List<MenuItem> rootMenu = new List<MenuItem>();
                    foreach (var kvp in dict)
                    {
                        List<MenuItem> menu = rootMenu;
                        MenuItem item = kvp.Value;
                        if (dict.ContainsKey(item.MenuPGuid))
                        {
                            menu = dict[item.MenuPGuid].items;
                            menu.Add(item);
                        }
                        else
                            menu.Add(item);
                    }

                    string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(rootMenu[0].items, Newtonsoft.Json.Formatting.Indented);

                    if (rootMenu.Count > 0)
                        return Ok(strJson);
                    else
                        return StatusCode(204);
                }
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 獲取多語言標題（輔助方法）
        /// </summary>
        private string GetCaption(string title, string langJson, string language)
        {
            try
            {
                if (string.IsNullOrEmpty(langJson))
                    return title;

                var langDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(langJson);
                if (langDict != null && langDict.ContainsKey(language))
                    return langDict[language];
            }
            catch
            {
                // 如果解析失敗，返回原始標題
            }

            return title;
        }

        /// <summary>
        /// 獲取頁面菜單項目
        /// </summary>
        [HttpGet]
        public ActionResult GetCMSMenus(string MenuGuid)
        {
            try
            {
                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@MenuGuid", MenuGuid);
                
                cmd.CommandText = @"
                    SELECT * 
                    FROM CMSMenus
                    WHERE MenuGuid = @MenuGuid 
                ";

                using DataSet ds = SqlHelper.ExecuteDataset(cmd);
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        #endregion

        #region 使用者管理 API

        /// <summary>
        /// 取得使用者個人資料
        /// </summary>
        [HttpGet]
        public ActionResult GetUserProfile(string UserGuid)
        {
            try
            {
                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@UserGuid", UserGuid);
                cmd.CommandText = @"
                    SELECT A.UserGuid, A.UserId, A.UserName, A.UserTitle, A.UserEmail, 
                           A.UserCellPhone, A.UserImageUrl, A.LastActiveDate,
                           B.DeptGuid, B.DeptName, B.DeptNameAll
                    FROM vw_CMSUser A
                    LEFT JOIN vw_BAT_OneCMSUserInOneCMSDept B ON A.UserGuid = B.UserGuid
                    WHERE A.UserGuid = @UserGuid";

                using DataSet ds = SqlHelper.ExecuteDataset(cmd);
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 取得使用者清單
        /// </summary>
        [HttpGet]
        public ActionResult GetUserList(string DeptGuid = "", string Keyword = "", int PageSize = 50, int PageIndex = 1)
        {
            try
            {
                using SqlCommand cmd = new SqlCommand();
                
                string whereClause = "WHERE 1=1";
                
                if (!string.IsNullOrEmpty(DeptGuid) && DeptGuid != _EmptyGuid)
                {
                    cmd.Parameters.AddWithValue("@DeptGuid", DeptGuid);
                    whereClause += " AND UserGuid IN (SELECT UserGuid FROM CMSUserInDept WHERE DeptGuid = @DeptGuid)";
                }

                if (!string.IsNullOrEmpty(Keyword))
                {
                    cmd.Parameters.AddWithValue("@Keyword", "%" + Keyword + "%");
                    whereClause += " AND (UserId LIKE @Keyword OR UserName LIKE @Keyword OR UserEmail LIKE @Keyword)";
                }

                cmd.CommandText = $@"
                    SELECT UserGuid, UserId, UserName, UserTitle, UserEmail, 
                           UserCellPhone, UserImageUrl, LastActiveDate, IsActive
                    FROM vw_CMSUser
                    {whereClause}
                    ORDER BY UserId
                    OFFSET {(PageIndex - 1) * PageSize} ROWS
                    FETCH NEXT {PageSize} ROWS ONLY";

                using DataSet ds = SqlHelper.ExecuteDataset(cmd);
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 更新使用者個人資料
        /// </summary>
        [HttpPost]
        public ActionResult UpdateUserProfile([FromBody] System.Text.Json.JsonElement obj)
        {
            try
            {
                string userGuid = obj.GetProperty("UserGuid").GetString();
                string userName = obj.GetProperty("UserName").GetString();
                string userTitle = Utility.CheckObjectKey(obj, "UserTitle") ? obj.GetProperty("UserTitle").GetString() : "";
                string userEmail = Utility.CheckObjectKey(obj, "UserEmail") ? obj.GetProperty("UserEmail").GetString() : "";
                string userCellPhone = Utility.CheckObjectKey(obj, "UserCellPhone") ? obj.GetProperty("UserCellPhone").GetString() : "";

                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@UserGuid", userGuid);
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@UserTitle", userTitle);
                cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                cmd.Parameters.AddWithValue("@UserCellPhone", userCellPhone);

                cmd.CommandText = @"
                    UPDATE CMSUser 
                    SET UserName = @UserName,
                        UserTitle = @UserTitle,
                        UserEmail = @UserEmail,
                        UserCellPhone = @UserCellPhone,
                        ModifiedDate = GETDATE()
                    WHERE UserGuid = @UserGuid";

                int affected = SqlHelper.ExecuteNonQuery(cmd);

                if (affected > 0)
                    return Ok(new { success = true, message = "更新成功" });
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        #endregion

        #region 部門管理 API

        /// <summary>
        /// 取得部門樹狀結構
        /// </summary>
        [HttpGet]
        public ActionResult GetDeptTree()
        {
            try
            {
                using DataSet ds = SqlHelper.ExecuteDataset(@"
                    SELECT DeptGuid, DeptPGuid, DeptID, DeptName, DeptNameAll, DeptOrder
                    FROM CMSDept
                    ORDER BY DeptNameAll");

                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 取得部門成員
        /// </summary>
        [HttpGet]
        public ActionResult GetDeptMembers(string DeptGuid)
        {
            try
            {
                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@DeptGuid", DeptGuid);
                cmd.CommandText = @"
                    SELECT A.UserGuid, B.UserId, B.UserName, B.UserTitle, 
                           A.DeptGuid, A.IsDeptAdmin
                    FROM CMSUserInDept A
                    INNER JOIN vw_CMSUser B ON A.UserGuid = B.UserGuid
                    WHERE A.DeptGuid = @DeptGuid
                    ORDER BY B.UserId";

                using DataSet ds = SqlHelper.ExecuteDataset(cmd);
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        #endregion

        #region 角色權限 API

        /// <summary>
        /// 取得角色清單
        /// </summary>
        [HttpGet]
        public ActionResult GetRoles()
        {
            try
            {
                using DataSet ds = SqlHelper.ExecuteDataset(@"
                    SELECT RoleGuid, RoleName, RoleDesc, IsActive
                    FROM CMSRole
                    WHERE IsActive = 1
                    ORDER BY RoleName");

                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 取得使用者的角色
        /// </summary>
        [HttpGet]
        public ActionResult GetUserRoles(string UserGuid)
        {
            try
            {
                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@UserGuid", UserGuid);
                cmd.CommandText = @"
                    SELECT A.RoleGuid, B.RoleName, B.RoleDesc
                    FROM CMSUserInRole A
                    INNER JOIN CMSRole B ON A.RoleGuid = B.RoleGuid
                    WHERE A.UserGuid = @UserGuid AND B.IsActive = 1
                    ORDER BY B.RoleName";

                using DataSet ds = SqlHelper.ExecuteDataset(cmd);
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 分配角色給使用者
        /// </summary>
        [HttpPost]
        public ActionResult AssignUserRole([FromBody] System.Text.Json.JsonElement obj)
        {
            try
            {
                string userGuid = obj.GetProperty("UserGuid").GetString();
                string roleGuid = obj.GetProperty("RoleGuid").GetString();
                
                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@UserGuid", userGuid);
                cmd.Parameters.AddWithValue("@RoleGuid", roleGuid);

                // 檢查是否已存在
                cmd.CommandText = @"
                    IF NOT EXISTS (SELECT 1 FROM CMSUserInRole WHERE UserGuid = @UserGuid AND RoleGuid = @RoleGuid)
                    BEGIN
                        INSERT INTO CMSUserInRole (UserGuid, RoleGuid)
                        VALUES (@UserGuid, @RoleGuid)
                        SELECT 1
                    END
                    ELSE
                        SELECT 0";

                int result = SqlHelper.ExecuteScalar<int>(cmd);

                if (result > 0)
                    return Ok(new { success = true, message = "角色分配成功" });
                else
                    return Ok(new { success = false, message = "角色已存在" });
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 移除使用者角色
        /// </summary>
        [HttpPost]
        public ActionResult RemoveUserRole([FromBody] System.Text.Json.JsonElement obj)
        {
            try
            {
                string userGuid = obj.GetProperty("UserGuid").GetString();
                string roleGuid = obj.GetProperty("RoleGuid").GetString();
                
                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@UserGuid", userGuid);
                cmd.Parameters.AddWithValue("@RoleGuid", roleGuid);

                cmd.CommandText = @"
                    DELETE FROM CMSUserInRole 
                    WHERE UserGuid = @UserGuid AND RoleGuid = @RoleGuid";

                int affected = SqlHelper.ExecuteNonQuery(cmd);

                if (affected > 0)
                    return Ok(new { success = true, message = "角色移除成功" });
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        #endregion

        #region 系統代碼 API

        /// <summary>
        /// 取得系統代碼
        /// </summary>
        [HttpGet]
        public ActionResult GetCMSCodes(string CodesType)
        {
            try
            {
                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@CodesType", CodesType);
                cmd.CommandText = @"
                    SELECT CodesGuid, CodesType, CodesID, CodesName, CodesDesc1, CodesDesc2, CodesDesc3
                    FROM RS_CMSCodes(@CodesType)
                    ORDER BY CodesDesc3, CodesID";

                using DataSet ds = SqlHelper.ExecuteDataset(cmd);
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 取得所有代碼類型
        /// </summary>
        [HttpGet]
        public ActionResult GetCodesTypes()
        {
            try
            {
                using DataSet ds = SqlHelper.ExecuteDataset(@"
                    SELECT DISTINCT CodesType
                    FROM CMSCodes
                    WHERE IsActive = 1
                    ORDER BY CodesType");

                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        #endregion

        #region 系統日誌 API

        /// <summary>
        /// 取得事件日誌
        /// </summary>
        [HttpGet]
        public ActionResult GetEventLog(string StartDate = "", string EndDate = "", string EventCode = "", int PageSize = 50, int PageIndex = 1)
        {
            try
            {
                using SqlCommand cmd = new SqlCommand();
                
                string whereClause = "WHERE 1=1";
                
                if (!string.IsNullOrEmpty(StartDate))
                {
                    cmd.Parameters.AddWithValue("@StartDate", DateTime.Parse(StartDate));
                    whereClause += " AND CreatedDate >= @StartDate";
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    cmd.Parameters.AddWithValue("@EndDate", DateTime.Parse(EndDate).AddDays(1));
                    whereClause += " AND CreatedDate < @EndDate";
                }

                if (!string.IsNullOrEmpty(EventCode))
                {
                    cmd.Parameters.AddWithValue("@EventCode", EventCode);
                    whereClause += " AND EventCode = @EventCode";
                }

                cmd.CommandText = $@"
                    SELECT EventGuid, EventCode, EventName, Parameter, 
                           CreatedUserID, CreatedDate
                    FROM CMSEventLog
                    {whereClause}
                    ORDER BY CreatedDate DESC
                    OFFSET {(PageIndex - 1) * PageSize} ROWS
                    FETCH NEXT {PageSize} ROWS ONLY";

                using DataSet ds = SqlHelper.ExecuteDataset(cmd);
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                if (ds.Tables[0].Rows.Count > 0)
                    return Ok(strJson);
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 記錄事件日誌
        /// </summary>
        [HttpPost]
        public ActionResult LogEvent([FromBody] System.Text.Json.JsonElement obj)
        {
            try
            {
                string eventCode = obj.GetProperty("EventCode").GetString();
                string eventName = obj.GetProperty("EventName").GetString();
                string parameter = Utility.CheckObjectKey(obj, "Parameter") ? obj.GetProperty("Parameter").GetString() : "";
                string createdUserID = Utility.CheckObjectKey(obj, "CreatedUserID") ? obj.GetProperty("CreatedUserID").GetString() : "";

                using SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"
                    INSERT INTO CMSEventLog (EventGuid, EventCode, EventName, Parameter, CreatedUserID, CreatedDate)
                    VALUES (NEWID(), @EventCode, @EventName, @Parameter, @CreatedUserID, GETDATE())";

                cmd.Parameters.AddWithValue("@EventCode", eventCode);
                cmd.Parameters.AddWithValue("@EventName", eventName);
                cmd.Parameters.AddWithValue("@Parameter", parameter);
                cmd.Parameters.AddWithValue("@CreatedUserID", createdUserID);

                SqlHelper.ExecuteNonQuery(cmd);

                return Ok(new { success = true, message = "日誌記錄成功" });
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        #endregion

        #region 選單日誌 API

        /// <summary>
        /// 記錄選單存取日誌
        /// </summary>
        [HttpPost]
        public ActionResult SetCMSMenuLog([FromBody] System.Text.Json.JsonElement obj)
        {
            try
            {
                string menuGuid = obj.GetProperty("MenuGuid").GetString();
                string userGuid = obj.GetProperty("UserGuid").GetString();

                using SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"
                    INSERT INTO CMSMenuLog (MenuLogGuid, MenuGuid, UserGuid, VisitDate)
                    VALUES (NEWID(), @MenuGuid, @UserGuid, GETDATE())";

                cmd.Parameters.AddWithValue("@MenuGuid", menuGuid);
                cmd.Parameters.AddWithValue("@UserGuid", userGuid);

                SqlHelper.ExecuteNonQuery(cmd);

                return Ok(new { success = true, message = "選單日誌記錄成功" });
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        #endregion
    }
}

