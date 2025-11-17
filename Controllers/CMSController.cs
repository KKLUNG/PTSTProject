using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;

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
        /// 獲取主菜單
        /// </summary>
        [HttpGet]
        public ActionResult GetCMSMainMenu(string UserGuid, string Language)
        {
            try
            {
                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@UserGuid", UserGuid);
                cmd.Parameters.AddWithValue("@Language", Language);
                
                cmd.CommandText = @"
                    SELECT * 
                    FROM vw_CMSMainMenu 
                    WHERE UserGuid = @UserGuid 
                    ORDER BY MenuSequence
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

