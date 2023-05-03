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
    public class ChangePasswordResourceFilter : IAsyncResourceFilter
    {
        private readonly UserManager<AkilliSayacUser> _userManager;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public ChangePasswordResourceFilter(UserManager<AkilliSayacUser> userManager, IUrlHelperFactory urlHelperFactory)
        {
            _userManager = userManager;
            _urlHelperFactory = urlHelperFactory;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(context);
            var redirectUrl = urlHelper.Content("~/Identity/Account/Manage/ChangePassword");
            var currentUrl = context.HttpContext.Request.Path;

            if (redirectUrl != currentUrl)
            {
                var user = await _userManager.GetUserAsync(context.HttpContext.User);
                if(user!= null)
                {
                    if (user.LastPasswordChangedDate.AddDays(90) < DateTime.Now)
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
