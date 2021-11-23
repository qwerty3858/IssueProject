//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Threading.Tasks;
//using IssueProject.Common;
//using IssueProject.Models;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//​
//namespace IssueProject.Configurations
//{
//    public static class AuthenticationConfigurations
//    {
//        public static void ConfigureAuthentication(this IServiceCollection services)
//        {
//            services.AddAuthentication(options =>
//            {
//                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
//            })
//                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions: null);
//​
//            services.AddTransient<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
//        }
//    }
//​
//    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
//    {
//        private readonly ILogger<ConfigureJwtBearerOptions> Log;
//​
//​
//        private readonly List<AuthClient> AuthClients;
//​
//        public ConfigureJwtBearerOptions(IConfiguration configuration, ILogger<ConfigureJwtBearerOptions> logger)
//        {
//            Log = logger;
//​
//            var vAuthClient = new AuthClient();
//            configuration.GetSection("AuthClient").Bind(vAuthClient);
//​
//            AuthClients = new List<AuthClient> { vAuthClient };
//        }
//​
//        public void Configure(string name, JwtBearerOptions options)
//        {
//            if (name != JwtBearerDefaults.AuthenticationScheme)
//                return;
//​
//            options.TokenValidationParameters = new TokenValidationParameters()
//            {
//                // The signing key must match!
//                ValidateIssuerSigningKey = true,
//                IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
//                {
//​
//                    var vKeys = new List<SecurityKey>();
//​
//                    vKeys.AddRange(AuthClients.Select(client => client.Secret).Select
//                        (Base64UrlTextEncoder.Decode)
//                        .Select(symmetricKeyAsBase64 => new SymmetricSecurityKey(symmetricKeyAsBase64))
//                    );
//​
//                    return vKeys;
//                },
//​
//                // Validate the JWT Issuer (iss) claim  
//                ValidateIssuer = true,
//                ValidIssuers = AuthClients.Select(Client => Client.Issuer).ToList(),
//​
//                // Validate the JWT Audience (aud) claim  
//                ValidateAudience = true,
//                ValidAudiences = AuthClients.Select(Client => Client.Id).ToList(),
//​
//                // Validate the token expiry  
//                ValidateLifetime = true,
//                LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
//                {
//                    JwtSecurityToken vToken = (JwtSecurityToken)securityToken;
//                    string vAudience = vToken.Audiences.First();
//​
//                    var vClient = AuthClients.FirstOrDefault(client => client.Id == vAudience);
//                    if (vClient == null)
//                    {

//                        Log.LogError("Client is null {0}", vAudience);
//                        return false;
//                    }
//​
//                    if (vClient.ValidateLifeTime)
//                    {
//                        if (validationParameters == null)
//                        {
//                            Log.LogError("IDX10000: The parameter '{0}' cannot be a 'null' or an empty object.", nameof(validationParameters));
//                            return false;
//                        }
//​
//                        if (!validationParameters.ValidateLifetime)
//                        {
//                            Log.LogError("IDX10238: ValidateLifetime property on ValidationParameters is set to false. Exiting without validating the lifetime.");
//                            return false;
//                        }
//​
//                        if (!expires.HasValue && validationParameters.RequireExpirationTime)
//                        {
//                            Log.LogError(" IDX10225: Lifetime validation failed. The token is missing an Expiration Time. Tokentype: '{0}'",
//                                securityToken == null ? "null" : securityToken.GetType().ToString());
//​
//                            return false;
//                        }
//​
//                        if (notBefore.HasValue && expires.HasValue && (notBefore.Value > expires.Value))
//                        {
//                            Log.LogTrace("IDX10224: Lifetime validation failed. The NotBefore: '{0}' is after Expires: '{1}'.", notBefore.Value, expires.Value);
//                            return false;
//                        }
//​
//                        DateTime vUtcNow = DateTime.UtcNow;
//                        if (notBefore.HasValue && (notBefore.Value > DateTimeUtil.Add(vUtcNow, validationParameters.ClockSkew)))
//                        {
//                            Log.LogTrace("IDX10222: Lifetime validation failed. The token is not yet valid. ValidFrom: '{0}', Current time: '{1}'.", notBefore.Value, vUtcNow);
//                            return false;
//                        }
//​
//                        if (expires.HasValue && (expires.Value < DateTimeUtil.Add(vUtcNow, validationParameters.ClockSkew.Negate())))
//                        {
//                            Log.LogTrace("IDX10223: Lifetime validation failed. The token is expired. ValidTo: '{0}', Current time: '{1}'.", expires.Value, vUtcNow);
//                            return false;
//                        }
//                    }
//                    //Açılmaz
//                    //Validators.ValidateLifetime(before, expires, token, parameters);
//​
//                    return true;
//                },
//​
//                ClockSkew = TimeSpan.Zero
//            };
//​
//            options.Events = new JwtBearerEvents
//            {
//                OnChallenge = OnChallenge
//            };
//        }
//​
//        private Task OnChallenge(JwtBearerChallengeContext context)
//        {
//            context.HandleResponse();
//​
//            var vErrorMessage = "Authentication Error";
//​
//            if (context.AuthenticateFailure != null)
//                if (context.AuthenticateFailure.GetType() == typeof(SecurityTokenValidationException))
//                {
//                    vErrorMessage += ": Invalid Token";
//                }
//                else if (context.AuthenticateFailure.GetType() ==
//                         typeof(SecurityTokenInvalidIssuerException))
//                {
//                    vErrorMessage += ": Invalid Issuer";
//                }
//                else if (context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
//                {
//                    vErrorMessage += ": Token Expired";
//                }
//                else if (context.AuthenticateFailure.GetType() ==
//                         typeof(SecurityTokenInvalidAudienceException))
//                {
//                    vErrorMessage += ": Invalid Audience";
//                }
//                else if (context.AuthenticateFailure.GetType() ==
//                         typeof(SecurityTokenInvalidSignatureException))
//                {
//                    vErrorMessage += ": Invalid Signature";
//                }
//​
//            Result vResponse = Result.PrepareFailure(CommonErrors.AuthenticationError);
//​
//            context.Response.StatusCode = 401;
//            context.Response.WriteAsync(vErrorMessage); //vResponse.ToJson(Formatting.Indented)
//​
//            return Task.CompletedTask;
//        }
//​
//        public void Configure(JwtBearerOptions options)
//        {
//            // default case: no scheme name was specified
//            Configure(string.Empty, options);
//        }
//    }
//}