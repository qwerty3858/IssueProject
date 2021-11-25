using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using IssueProject.Entity.Context;
using IssueProject.Helpers.TokenOperations;
using IssueProject.Entity;
using IssueProject.Common;
using IssueProject.Models.User;
using Microsoft.Extensions.Logging;

namespace IssueProject.Services
{
    public class AuthService
    {
        //Member Variables/////////////////////////////////////////////////////

        private readonly _2Mes_ConceptualContext Db;
        private readonly IConfiguration Configuration;
        private ILogger<AuthService> _logger;
        //Constructor//////////////////////////////////////////////////////////

        public AuthService(ILogger<AuthService> logger ,IConfiguration configuration, _2Mes_ConceptualContext db)
        {
            Configuration = configuration;
            Db = db;
            _logger = logger;
        }

        //Public Functions/////////////////////////////////////////////////////

        public async Task<Result<LoginInfo>> AuthForLogIn(LoginQuery loginQuery)
        {
            return loginQuery.GrantType switch
            {
                "password" => await GrantWithPassword(loginQuery),
                "refresh_token" => await GrantWithRefreshToken(loginQuery),
                _ => Result<LoginInfo>.PrepareFailure("Giriş için yanlış parametre gönderildi")
            };
        }

        //Private Functions////////////////////////////////////////////////////

        private async Task<Result<LoginInfo>> GrantWithPassword(LoginQuery loginQuery)
        {
            try
            {
            var vClient = new AuthClient();
            Configuration.GetSection("AuthClient").Bind(vClient);

            //Kullanıcı Kontrolü ve Token
            ///////////////////////////////////////////////////////////////////////////////////////

            User vUser = await Db.Users
                .Include(user => user.Role)
                .Where(User => User.Id == loginQuery.UserId)
                .FirstOrDefaultAsync();

            if (vUser == null)
                return Result<LoginInfo>.PrepareFailure("Kullanıcı Bulunamadı!");

            if (vUser.Password != loginQuery.Password)
                return Result<LoginInfo>.PrepareFailure("Girilen Şifre Hatalı");          

            var vIdentity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);

            vIdentity.AddClaim(new Claim("aud", vClient.Id));
            vIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, vUser.Id.ToString()));
            vIdentity.AddClaim(new Claim(ClaimTypes.Role, vUser.Role.Definition));

            DateTime vIssuedTime = DateTime.UtcNow;

            string vToken = PrepareToken(vClient, vIdentity, vIssuedTime);

            //RefreshToken Oluşturma
            ///////////////////////////////////////////////////////////////////////////////////////

            string vRefreshTokenId = Guid.NewGuid().ToString("n");
            var vRefreshToken = new UserToken()
            {
                Id = vRefreshTokenId.GetHash(),
                ClientId = vClient.Id,
                Subject = vUser.Id.ToString(),
                IssuedUtc = vIssuedTime,
                ExpiresUtc = vIssuedTime.Date.AddMinutes(Convert.ToDouble(vClient.RefreshTokenLifeTime))
            };

            var vTicket = new AuthenticationTicket(new ClaimsPrincipal(vIdentity), new AuthenticationProperties(),
                JwtBearerDefaults.AuthenticationScheme)
            {
                Properties =
                {
                    IssuedUtc = vRefreshToken.IssuedUtc,
                    ExpiresUtc = vRefreshToken.ExpiresUtc
                }
            };

            var vSerializer = new TicketSerializer();
            vRefreshToken.ProtectedTicket = Convert.ToBase64String(vSerializer.Serialize(vTicket));

            //Refresh token daha sonra işlem yapılabilmesi için veritabanına kaydediliyor ve Token'a atanıyor
            ///////////////////////////////////////////////////////////////////////////////////////

            UserToken vPreviousToken = Db.UserTokens.FirstOrDefault(token =>
                token.Subject == vRefreshToken.Subject && token.ClientId == vRefreshToken.ClientId);

            if (vPreviousToken != null)
                Db.UserTokens.Remove(vPreviousToken);

            await Db.UserTokens.AddAsync(vRefreshToken);
            await Db.SaveChangesAsync();

            //Cevap oluşturuluyor
            ///////////////////////////////////////////////////////////////////////////////////////

            int vTokenLifeTime = vClient.TokenLifeTime * 60;
            var vLoginInfo = new LoginInfo
            {
                UserId = vUser.Id,
                FullName = vUser.FullName,
                Role = vUser.RoleId,
                AccessToken = vToken,
                RefreshToken = vRefreshTokenId,
                TokenType = JwtBearerDefaults.AuthenticationScheme,
                ValidFor = vTokenLifeTime - 1,
                Issued = vIssuedTime.ToUniversalTime(),
                Expires = vIssuedTime.AddSeconds(vTokenLifeTime).ToUniversalTime()
            };

            return Result<LoginInfo>.PrepareSuccess(vLoginInfo);

            }
            catch(Exception vEx)
            {
                _logger.LogInformation($"Auth Login Error: {vEx.Message}");
                return Result<LoginInfo>.PrepareFailure(vEx.Message);
            }
           
        }

        private async Task<Result<LoginInfo>> GrantWithRefreshToken(LoginQuery loginQuery)
        {
            var vClient = new AuthClient();
            Configuration.GetSection("AuthClient").Bind(vClient);

            // Kaydedilmiş ticket bulunuyor
            //*********************************************************************************************************

            string vHashedTokenId = loginQuery.RefreshToken.GetHash();

            UserToken vUserToken = await Db.UserTokens.FirstOrDefaultAsync(token => token.Id == vHashedTokenId);
            if (vUserToken == null)
                return Result<LoginInfo>.PrepareFailure("Token bulunamadı");


            User vUser = await Db.Users.FindAsync(Convert.ToInt32(vUserToken.Subject));
            if (vUser == null)
                return Result<LoginInfo>.PrepareFailure("Kullanıcı bulunamadı");

            AuthenticationTicket vTicket =
                new TicketSerializer().Deserialize(Convert.FromBase64String(vUserToken.ProtectedTicket));

            //RefreshToken zamanı doldu ise veritabanından siliniyor
            if (vUserToken.ExpiresUtc < DateTime.UtcNow)
            {
                Db.UserTokens.Remove(vUserToken);
                await Db.SaveChangesAsync();

                return Result<LoginInfo>.PrepareFailure("Oturumun süresi dolduğu için yeniden giriş yapmalısınız");
            }

            ClaimsIdentity vIdentity = vTicket.Principal.Identities.FirstOrDefault();
            if (vIdentity == null)
                return Result<LoginInfo>.PrepareFailure("Oturum kimlik hatası");

            // Kaydedilmiş ticketten tekrar token elde ediliyor
            //*********************************************************************************************************

            DateTime vIssuedTime = DateTime.UtcNow;

            string vToken = PrepareToken(vClient, vIdentity, vIssuedTime);

            var vLoginInfo = new LoginInfo
            {
                UserId = vUser.Id,
                FullName = vUser.FullName,
                Role = vUser.RoleId,
                AccessToken = vToken,
                RefreshToken = loginQuery.RefreshToken,
                TokenType = JwtBearerDefaults.AuthenticationScheme,
                ValidFor = (vClient.TokenLifeTime * 60) - 1,
                Issued = vIssuedTime.ToUniversalTime(),
                Expires = vIssuedTime.AddMinutes(vClient.TokenLifeTime).ToUniversalTime()
            };

            return Result<LoginInfo>.PrepareSuccess(vLoginInfo);
        }

        private string PrepareToken(AuthClient client, ClaimsIdentity identity, DateTime issuedTime,
            DateTime? expiresTime = null)
        {
            var vClientId = "";
            if (identity.HasClaim(claim => claim.Type == "aud"))
                vClientId = identity.Claims.GetValue("aud");

            if (vClientId == "")
                throw new ArgumentException("Geçersiz istemci id");

            if (vClientId != client.Id)
                throw new ArgumentException("İstemci aynı değil");

            byte[] vSymmetricKeyAsBase64 = Base64UrlTextEncoder.Decode(client.Secret);
            var vSecurityKey = new SymmetricSecurityKey(vSymmetricKeyAsBase64) { KeyId = client.KeyId };
            var vSigningKey = new SigningCredentials(vSecurityKey, SecurityAlgorithms.HmacSha256);

            DateTime vIssued = issuedTime;
            DateTime vExpires = expiresTime ?? issuedTime.AddYears(1); //client.TokenLifeTime);

            if (vIssued > vExpires)
                vIssued = vExpires.AddDays(-1);

            var vTokenHandler = new JwtSecurityTokenHandler();
            vTokenHandler.OutboundAlgorithmMap.Clear();

            return vTokenHandler.CreateEncodedJwt(
                client.Issuer,
                null,
                identity,
                vIssued,
                vExpires,
                issuedTime,
                vSigningKey);
        }
    }
}