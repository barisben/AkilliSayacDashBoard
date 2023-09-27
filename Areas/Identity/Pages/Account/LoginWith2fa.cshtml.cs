// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AkilliSayac.Areas.Identity.Data;
using AkilliSayac.Models;
using AkilliSayac.Data;

namespace AkilliSayac.Areas.Identity.Pages.Account
{
    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<AkilliSayacUser> _signInManager;
        private readonly UserManager<AkilliSayacUser> _userManager;
        private readonly ILogger<LoginWith2faModel> _logger;
        private readonly ApplicationDbContext _db;

        public LoginWith2faModel(
            SignInManager<AkilliSayacUser> signInManager,
            UserManager<AkilliSayacUser> userManager,
            ApplicationDbContext db,
            ILogger<LoginWith2faModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _db = db;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator kodu")]
            public string TwoFactorCode { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            ReturnUrl = returnUrl;
            RememberMe = false;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, false);

            var userId = await _userManager.GetUserIdAsync(user);

            Log log = new Log();
            log.LogTime = DateTime.Now;
            log.LogTypeId = _db.LogTypes.Where(x => x.LogTypeName == "User").FirstOrDefault().LogTypeId;
            log.UserId = user.Id;

            if (result.Succeeded)
            {
                user.LastLoginDate = DateTime.Now;
                await _signInManager.UserManager.UpdateAsync(user);

                log.LogMessage = "Kullanıcı, 2fa doğrulama kodu ile başarılı giriş yaptı.";
                log.LogStatusBadge = "badge bg-success";
                log.LogStatus = "Başarılı";


                _db.Logs.Add(log);
                _db.SaveChanges();

                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                log.LogMessage = "Kullanıcının hesabı 2fa sırasında bloke oldu.";
                log.LogStatusBadge = "badge bg-danger";
                log.LogStatus = "Uyarı";

                _db.Logs.Add(log);
                _db.SaveChanges();

                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToPage("./Lockout");
            }
            else
            {
                log.LogMessage = "Kullanıcı, 2fa doğrulama kodunu hatalı girdi.";
                log.LogStatusBadge = "badge bg-danger";
                log.LogStatus = "Uyarı";

                _db.Logs.Add(log);
                _db.SaveChanges();

                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Doğrulama kodu hatalı.");
                return Page();
            }
        }
    }
}
