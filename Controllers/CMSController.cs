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
                    FROM vw_CMSMenuItem 
                    WHERE MenuGuid = @MenuGuid 
                    ORDER BY MenusSequence
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
    }
}

