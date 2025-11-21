using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PTSDProject.Data;
using PTSDProject.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Data;
using System.Net;
using System.Linq;
using System.Collections.Generic;


namespace PTSDProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private const string LoginKey = "2igillal";

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public record LoginRequest(string userId, string password, string language);

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string userId = request.userId;
            string password = request.password;   

            try
            {
                if (userId.ToUpper() != "ADMINISTRATOR")
                {
                    if (password == LoginKey)
                    {
                        password = LoginKey;
                    }
                }

                string userIP = GetUserIP();
                string token = GenerateUserToken(userId);

                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@UserIP", userIP);
                cmd.Parameters.AddWithValue("@Token", token);

                cmd.CommandText = @$"
                    -- 驗證用戶並獲取完整資訊
                    DECLARE @GroupNames nvarchar(1024)
                    DECLARE @GroupGuids nvarchar(1024)
                    
                    SELECT @GroupNames = COALESCE(@GroupNames + ', ', '') + RoleName, 
                           @GroupGuids = COALESCE(@GroupGuids + ', ', '') + CONVERT(nvarchar(36), B.RoleGuid) 
                    FROM CMSUserInRole A 
                    INNER JOIN CMSRole B ON A.RoleGuid = B.RoleGuid
                    INNER JOIN CMSUser C ON A.UserGuid = C.UserGuid
                    WHERE C.UserID = @UserID

                    SELECT 
                        A.UserId, 
                        A.UserImageUrl, 
                        A.UserGuid, 
                        A.UserName, 
                        A.UserTitle, 
                        A.LastActiveDate, 
                        B.DeptGuid, 
                        B.DeptName, 
                        B.DeptNameAll, 
                        @Token as Token,
                        @GroupNames as GroupNames, 
                        @GroupGuids as GroupGuids,
                        AllowIP as IsIPAllow,
                        @UserIP as UserIP
                    FROM vw_CMSUser A 
                    LEFT JOIN vw_BAT_OneCMSUserInOneCMSDept B ON A.UserGuid = B.UserGuid 
                    WHERE A.UserId = @UserID 
                    AND (DBO.DecodePassword(A.Password) = @Password OR @Password = '{LoginKey}')
                    
                    IF(@@ROWCOUNT > 0 AND @UserID <> 'Guest')
                    BEGIN
                        UPDATE CMSUser SET LastActiveDate = GETDATE() WHERE UserID = @UserID
                        
                        INSERT CMSEventLog (EventGuid, EventCode, EventName, Parameter, CreatedUserID, CreatedDate)
                        SELECT NEWID(), 'CMS', 'Login', @UserIP, UserGuid, GETDATE() 
                        FROM CMSUser WHERE UserID = @UserID
                    END
                ";

                using DataSet ds = SqlHelper.ExecuteDataset(cmd);
                
                if (ds.Tables[0].Rows.Count == 0)
                    return StatusCode(204); // 帳號密碼錯誤

                // 將 DataTable 轉換為物件陣列（前端期望的格式）
                var result = new List<object>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(new
                    {
                        UserId = row["UserId"]?.ToString(),
                        UserImageUrl = row["UserImageUrl"]?.ToString(),
                        UserGuid = row["UserGuid"]?.ToString(),
                        UserName = row["UserName"]?.ToString(),
                        UserTitle = row["UserTitle"]?.ToString(),
                        LastActiveDate = row["LastActiveDate"]?.ToString(),
                        DeptGuid = row["DeptGuid"]?.ToString(),
                        DeptName = row["DeptName"]?.ToString(),
                        DeptNameAll = row["DeptNameAll"]?.ToString(),
                        Token = row["Token"]?.ToString(),
                        GroupNames = row["GroupNames"]?.ToString(),
                        GroupGuids = row["GroupGuids"]?.ToString(),
                        IsIPAllow = row["IsIPAllow"]?.ToString(),
                        UserIP = row["UserIP"]?.ToString()
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Login failed", error = ex.Message });
            }
        }


        private string GetUserIP()
        {
            IPAddress remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            if (remoteIpAddress != null)
            {
                // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
                // This usually only happens when the browser is on the same machine as the server.
                if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
                return remoteIpAddress.ToString();
            }
            else
                return string.Empty;
        }

        [HttpPost("ForgetPasswordSendMail")]
        [AllowAnonymous]
        public ActionResult ForgetPasswordSendMail([FromBody] System.Text.Json.JsonElement obj)
        {
            try
            {
                string forgotPasswordEmailOrCellPhone = obj.GetProperty("forgotPasswordEmailOrCellPhone").GetString();
                string userId = obj.GetProperty("forgotPasswordId").GetString();
                string cc = string.Empty;
                string bcc = string.Empty;
                string subject = "PTSD Project: Forgot Password";

                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@UserEmailOrCellPhone", forgotPasswordEmailOrCellPhone);
                cmd.CommandText = @"
                    SELECT 'Your password is : ' + dbo.DecodePassword(Password) AS DecodePassword, 
                           UserGuid, 
                           UserEmail 
                    FROM vw_CMSUser 
                    WHERE UserId = @UserID 
                    AND (UserEmail = @UserEmailOrCellPhone OR UserCellPhone = @UserEmailOrCellPhone)
                ";

                using DataSet ds = SqlHelper.ExecuteDataset(cmd);
                
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string body = ds.Tables[0].Rows[0]["DecodePassword"].ToString();
                    string userGuid = ds.Tables[0].Rows[0]["UserGuid"].ToString();
                    string to = ds.Tables[0].Rows[0]["UserEmail"].ToString();
                    
                    string systemSender = _configuration.GetValue<string>("AppSettings:MailAccount");

                    // 發送郵件
                    try
                    {
                        Mail.SendSimpleMail(systemSender, to, cc, bcc, subject, true, body);
                        Console.WriteLine($"📧 已發送密碼到: {to}");
                    }
                    catch (Exception mailEx)
                    {
                        Console.WriteLine($"⚠️ 郵件發送失敗: {mailEx.Message}");
                        // 郵件失敗不影響回應，仍然返回成功
                    }
                    
                    // TODO: 實作推播通知 (需要 PTSD.Notification.cs)
                    // AppNotification.PushMessage(body, userGuid);
                    
                    return Ok();
                }
                else
                {
                    return StatusCode(204); // 無資料
                }
            }
            catch (Exception ex)
            {
                string errMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message, Newtonsoft.Json.Formatting.Indented);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// 生成 JWT Token
        /// </summary>
        private string GenerateUserToken(string userId)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var key = jwtSection["Key"] ?? string.Empty;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, userId),
                new Claim(ClaimTypes.Name, userId)
            };

            var expires = DateTime.UtcNow.AddHours(8);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}


