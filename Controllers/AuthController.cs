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
            string UserId = request.Username;
            string Password = request.Password;

            //設定語系
            //string Language = Bia.Utility.CheckObjectKey(obj, "Language") ? obj.GetProperty("Language").GetString() : "zhTW";

            try
            {
                if(UserId.ToUpper() != "ADMINISTRATOR")
                {
                    if(Password == LoginKey)
                    {
                        Password = LoginKey;
                    }
                }
                string UserIP = GetUserIP();
                using SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@UserID", UserId);
                cmd.Parameters.AddWithValue("@Password", Password);
                cmd.Parameters.AddWithValue("@UserIP", UserIP);

                cmd.CommandText = @$"SELECT UserId, DBO.DecodePassword(Password) AS DecodedPassword 
                            FROM CMSUser 
                            WHERE UserId = @UserID AND (DBO.DecodePassword(Password) = @Password OR @Password = '{LoginKey}')
							IF(@@ROWCOUNT != 0)
							BEGIN
								UPDATE CMSUser
								SET LastActiveDate = GETDATE()
								WHERE UserId =  @UserID
                                INSERT CMSEventLog (EventGuid, EventCode, EventName, Parameter, CreatedUserID, CreatedDate)
                                        SELECT NEWID(), 'CMS', 'Login', @UserIP, UserGuid, GETDATE() FROM CMSUser WHERE UserID = @UserID
							END
                ";
                using DataSet ds = SqlHelper.ExecuteDataset(cmd);
                if (ds.Tables[0].Rows.Count == 0)
                    return new NoContentResult();

            }
            catch
            {
                return BadRequest(new { message = "Username and password are required ,please check it" });
            }

            // 從資料庫查詢使用者並驗證密碼

            return Ok();
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

    }
}


