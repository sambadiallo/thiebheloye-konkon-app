using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using Thiebheloye.Identity.Data.EF.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using Thiebheloye.Domain.Core.Interfaces;
using Thiebheloye.Domain.Core.Interfaces.Adapters;
using Thiebheloye.Domain.Core.Models.Identity;
using Thiebheloye.Domain.Core.Models;
using Thiebheloye.Identity.Data.EF.Providers;

namespace Thiebheloye.Identity.Data.EF.Adapters
{
    public class AccountAdapter : IAccountAdapter
    {
        private const string LocalLoginProvider = "Local";
        private readonly UserManager _userManager;
        private readonly SignInManager _signInManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ISecureDataFormat<AuthenticationTicket> _accessTokenFormat;

        public AccountAdapter(UserManager userManager,
       ISecureDataFormat<AuthenticationTicket> accessTokenFormat,
            IAuthenticationManager authenticationManager,
            SignInManager signInManager
            )
        {
            _userManager = userManager;
            _authenticationManager = authenticationManager;
            _signInManager = signInManager;
            _accessTokenFormat = accessTokenFormat;
        }



        // POST api/Account/Register

        public async Task<CommandResult> Register(RegisterCommandArguments model)
        {
            var result = new CommandResult();
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                MapErrors(createResult, result);
            }

            return result;
        }

        // POST api/Account/Logout

        public CommandResult Logout()
        {
            _authenticationManager.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return new CommandResult();
        }

        public async Task<CommandResult> Login(LoginCommandArguments arguments)
        {
            var result = new CommandResult();
            var signInResult = await _signInManager.PasswordSignInAsync(arguments.Email, arguments.Password, true, true);

            if (signInResult != SignInStatus.Success)
            {
                result.FailureDetails.Add(new FailureDetail
                {
                    Reason = FailureReason.SignInFailure,
                    StatusCode = StatusCode.NotAuthorized,
                    Information = $"{signInResult}"
                });
            }

            return result;
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await _userManager.FindByIdAsync(_authenticationManager.User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        public async Task<CommandResult> ChangePassword(ChangePasswordBindingModel model)
        {


            IdentityResult result = await _userManager.ChangePasswordAsync(_authenticationManager.User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                // return GetErrorResult(result);
            }

            return new CommandResult();
        }

        // POST api/Account/SetPassword
        public async Task<IResult<IdentityResult>> SetPassword(SetPasswordBindingModel model)
        {

            IdentityResult result = await _userManager.AddPasswordAsync(_authenticationManager.User.Identity.GetUserId(), model.NewPassword);
            return new CommandResult<IdentityResult>() { Response = result };
        }

        // POST api/Account/AddExternalLogin
        public async Task<CommandResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {

            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = _accessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
            && ticket.Properties.ExpiresUtc.HasValue
            && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                //return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                // return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await _userManager.AddLoginAsync(_authenticationManager.User.Identity.GetUserId(),
            new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));


            return new CommandResult();
        }

        // POST api/Account/RemoveLogin
        public async Task<IResult> RemoveLogin(RemoveLoginBindingModel model)
        {


            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await _userManager.RemovePasswordAsync(_authenticationManager.User.Identity.GetUserId());
            }
            else
            {
                result = await _userManager.RemoveLoginAsync(_authenticationManager.User.Identity.GetUserId(), new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            return new CommandResult();
        }

        // GET api/Account/ExternalLogin

        public async Task<IResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                // return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            //if (!User.Identity.IsAuthenticated)
            //{
            //    // return new ChallengeResult(provider, this);
            //}

            ExternalLoginData externalLogin = new ExternalLoginData(); //ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                // return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                // return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await _userManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
            externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(_userManager,
                OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(_userManager,
                CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                _authenticationManager.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                _authenticationManager.SignIn(identity);
            }

            return new CommandResult();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true

        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = _authenticationManager.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                //ExternalLoginViewModel login = new ExternalLoginViewModel
                //{
                //    Name = description.Caption,
                //    Url = Url.Route("ExternalLogin", new
                //    {
                //        provider = description.AuthenticationType,
                //        response_type = "token",
                //        client_id = Startup.PublicClientId,
                //        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                //        state = state
                //    }),
                //    State = state
                //};
                //logins.Add(login);
            }

            return logins;
        }


        // POST api/Account/RegisterExternal

        public async Task<CommandResult> RegisterExternal(RegisterExternalBindingModel model)
        {


            var info = await _authenticationManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                // return GetErrorResult(result);
            }

            result = await _userManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                // return GetErrorResult(result);
            }
            return new CommandResult();
        }

        #region Helpers


        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        private static void MapErrors(IdentityResult createResult, IResult result)
        {
            foreach (var errorMessage in createResult.Errors)
            {
                result.FailureDetails.Add(new FailureDetail()
                {
                    StatusCode = StatusCode.UserCreating,
                    Reason = FailureReason.Authorization,
                    Information = errorMessage
                });
            }
        }

        #endregion
    }
}
