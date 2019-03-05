using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolicyLDAP.Identity
{
    public class LdapSignInManager : SignInManager<LdapUser>
    {
        public LdapSignInManager(
            LdapUserManager ldapUserManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<LdapUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<LdapSignInManager> logger,
            IAuthenticationSchemeProvider schemes)
            : base(
                ldapUserManager,
                contextAccessor,
                claimsFactory,
                optionsAccessor,
                logger,
                schemes)
        {
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe, bool lockOutOnFailure)
        {
            var user = await UserManager.FindByNameAsync(userName);

            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await PasswordSignInAsync(user, password, rememberMe, lockOutOnFailure);
        }
    }
}
