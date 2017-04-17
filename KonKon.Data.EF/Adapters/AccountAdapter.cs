﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.ModelBinding;
using DryIoc;
using KonKon.Data.EF.Models.Identity;
using KonKon.Domain.Core.Interfaces.Adapters;
using KonKon.Domain.Core.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using KonKon.Domain.Core.Interfaces;
using KonKon.Domain.Core.Models;
using KonKon.Mobile.WebApi.Providers;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;

namespace KonKon.Data.EF.Adapters
{
    public class AccountAdapter : IAccountAdapter
    {
        private const string LocalLoginProvider = "Local";
        private readonly ApplicationUserManager _userManager;
        private readonly IAuthenticationManager _authenticationManager;

        public AccountAdapter(ApplicationUserManager userManager,
        ISecureDataFormat<AuthenticationTicket> accessTokenFormat,
            IAuthenticationManager authenticationManager)
        {
            _userManager = userManager;
            _authenticationManager = authenticationManager;

            AccessTokenFormat = accessTokenFormat;
        }



        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo


        // POST api/Account/Logout

        public IResult Logout()
        {
            _authenticationManager.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return new CommandResult();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        public async Task<ManageInfoViewModel> GetManageInfo(string userId, string returnUrl, bool generateState = false)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userId);

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
        public async Task<IResult> ChangePassword(string userId, ChangePasswordBindingModel model)
        {


            IdentityResult result = await _userManager.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                // return GetErrorResult(result);
            }

            return new CommandResult();
        }

        // POST api/Account/SetPassword
        public async Task<IResult<IdentityResult>> SetPassword(string userId, SetPasswordBindingModel model)
        {

            IdentityResult result = await _userManager.AddPasswordAsync(userId, model.NewPassword);
            return new CommandResult<IdentityResult>() { Response = result };
        }

        // POST api/Account/AddExternalLogin
        public async Task<IResult> AddExternalLogin(string userId, AddExternalLoginBindingModel model)
        {

            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

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

            IdentityResult result = await _userManager.AddLoginAsync(userId,
            new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));


            return new CommandResult();
        }

        // POST api/Account/RemoveLogin
        public async Task<IResult> RemoveLogin(string userId, RemoveLoginBindingModel model)
        {


            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await _userManager.RemovePasswordAsync(userId);
            }
            else
            {
                result = await _userManager.RemoveLoginAsync(userId, new UserLoginInfo(model.LoginProvider, model.ProviderKey));
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

        // POST api/Account/Register

        public async Task<IResult> Register(RegisterBindingModel model)
        {


            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                //return GetErrorResult(result);
            }

            return new CommandResult();
        }

        // POST api/Account/RegisterExternal

        public async Task<IResult> RegisterExternal(RegisterExternalBindingModel model)
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

        #endregion
    }
}
