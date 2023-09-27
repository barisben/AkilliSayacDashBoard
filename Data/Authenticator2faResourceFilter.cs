using AkilliSayac.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace AkilliSayac.Data
{
    [Authorize]
    public class AuthenticatorResourceFilter2fa : IAsyncResourceFilter
    {
        private readonly UserManager<AkilliSayacUser> _userManager;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public AuthenticatorResourceFilter2fa(UserManager<AkilliSayacUser> userManager, IUrlHelperFactory urlHelperFactory)
        {
            _userManager = userManager;
            _urlHelperFactory = urlHelperFactory;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(context);
            var redirectUrl = urlHelper.Content("~/Identity/Account/Manage/EnableAuthenticator");
            var logoutUrl = urlHelper.Content("~/Identity/Account/Logout");
            var currentUrl = context.HttpContext.Request.Path;

            if (redirectUrl != currentUrl && currentUrl != logoutUrl)
            {
                var user = await _userManager.GetUserAsync(context.HttpContext.User);
                if(user!= null)
                {
                    if (user.TwoFactorEnabled == false)
                    {
                        context.Result = new RedirectResult(redirectUrl);
                        return;
                    }
                }
            }

            await next();
        }
    }
}
