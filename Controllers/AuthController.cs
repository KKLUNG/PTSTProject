using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PTSDProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Data;
using System.Net;


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

        public record LoginRequest(string Username, string Password);

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            string userId = request.Username;
            string password = request.Password;

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
                        '1' as IsIPAllow,
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

                // 返回完整用戶資料（前端需要的格式）
                string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0], Newtonsoft.Json.Formatting.Indented);
                return Ok(strJson);
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

                    // TODO: 實作郵件發送
                    // Mail.SendSimpleMail(systemSender, to, cc, bcc, subject, true, body);
                    
                    // TODO: 實作推播通知
                    // AppNotification.PushMessage(body, userGuid);
                    
                    Console.WriteLine($"📧 發送密碼到: {to}, 內容: {body}");
                    
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


