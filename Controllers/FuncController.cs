using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;

namespace PTSDProject.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class FuncController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private const string _emptyGuid = "00000000-0000-0000-0000-000000000000";
        private const string _defaultUploadFolder = "/upload/";

        public FuncController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        #region 文件處理

        /// <summary>
        /// 文件上傳
        /// </summary>
        [HttpPost]
        public ActionResult FileUpload(string FileGuid, string UploadPath)
        {
            try
            {
                string rtnPath = string.Empty;
                var myFile = Request.Form.Files[0];
                string uploadFolder = string.IsNullOrEmpty(UploadPath) ? _defaultUploadFolder : UploadPath;
                rtnPath = uploadFolder;

                string webRootPath = _hostingEnvironment.WebRootPath;
                string contentRootPath = _hostingEnvironment.ContentRootPath;
                string targetFolder = Path.Combine(webRootPath, uploadFolder.TrimStart('/'));

                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                if (myFile.Length > 0)
                {
                    string fileExt = Path.GetExtension(myFile.FileName);
                    string newFileName = string.IsNullOrEmpty(FileGuid) ? Guid.NewGuid().ToString() : FileGuid;
                    string fullPath = Path.Combine(targetFolder, newFileName + fileExt);
                    
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        myFile.CopyTo(stream);
                    }

                    rtnPath += newFileName + fileExt;
                }

                return Ok(new { filePath = rtnPath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Upload failed", error = ex.Message });
            }
        }

        /// <summary>
        /// 文件刪除
        /// </summary>
        [HttpPost]
        public ActionResult FileDelete([FromBody] System.Text.Json.JsonElement obj)
        {
            try
            {
                string filePath = obj.GetProperty("FilePath").GetString();
                
                if (string.IsNullOrEmpty(filePath))
                    return BadRequest(new { message = "FilePath is required" });

                string webRootPath = _hostingEnvironment.WebRootPath;
                string fullPath = Path.Combine(webRootPath, filePath.TrimStart('/'));

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    return Ok(new { message = "File deleted successfully" });
                }
                else
                {
                    return StatusCode(204); // 文件不存在
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Delete failed", error = ex.Message });
            }
        }

        #endregion

        #region 工具函數

        /// <summary>
        /// 產生新的 GUID
        /// </summary>
        [HttpGet]
        public ActionResult GetNewGuid()
        {
            return Ok(new { guid = Guid.NewGuid().ToString() });
        }

        /// <summary>
        /// 取得未讀訊息數量 (Badge)
        /// </summary>
        [HttpGet]
        public ActionResult GetBadge(string UserGuid)
        {
            try
            {
                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@UserGuid", UserGuid);
                
                cmd.CommandText = @"
                    SELECT ISNULL(COUNT(*), 0) AS Badge
                    FROM CMSNotify
                    WHERE ToUserGuid = @UserGuid 
                    AND IsRead = 0 
                    AND IsActive = 1";

                using DataSet ds = SqlHelper.ExecuteDataset(cmd);
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);

                return Ok(strJson);
            }
            catch (Exception ex)
            {
                // 如果資料表不存在，返回 0
                return Ok("[{\"Badge\": 0}]");
            }
        }

        #endregion
    }
}

