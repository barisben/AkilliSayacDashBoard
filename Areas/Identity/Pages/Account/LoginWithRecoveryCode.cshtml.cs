// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AkilliSayac.Areas.Identity.Data;
using AkilliSayac.Data;
using AkilliSayac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
namespace AkilliSayac.Areas.Identity.Pages.Account
{
    public class LoginWithRecoveryCodeModel : PageModel
    {
        private readonly SignInManager<AkilliSayacUser> _signInManager;
        private readonly UserManager<AkilliSayacUser> _userManager;
        private readonly ILogger<LoginWithRecoveryCodeModel> _logger;
        private readonly ApplicationDbContext _db;

        public LoginWithRecoveryCodeModel(
            SignInManager<AkilliSayacUser> signInManager,
            UserManager<AkilliSayacUser> userManager,
            ILogger<LoginWithRecoveryCodeModel> logger,
            ApplicationDbContext db)
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
            [BindProperty]
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Kurtarma Kodu")]
            public string RecoveryCode { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            var userId = await _userManager.GetUserIdAsync(user);

            Log log = new Log();
            log.LogTime = DateTime.Now;
            log.LogTypeId = _db.LogTypes.Where(x => x.LogTypeName == "User").FirstOrDefault().LogTypeId;
            log.UserId = user.Id;

            if (result.Succeeded)
            {
                user.LastLoginDate = DateTime.Now;
                await _signInManager.UserManager.UpdateAsync(user);

                log.LogMessage = "Kullanıcı, 2fa kurtarma kodu ile başarılı giriş yaptı.";
                log.LogStatusBadge = "badge bg-success";
                log.LogStatus = "Başarılı";


                _db.Logs.Add(log);
                _db.SaveChanges();

                _logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            if (result.IsLockedOut)
            {
                log.LogMessage = "Kullanıcının hesabı 2fa sırasında bloke oldu.";
                log.LogStatusBadge = "badge bg-danger";
                log.LogStatus = "Uyarı";

                _db.Logs.Add(log);
                _db.SaveChanges();

                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                log.LogMessage = "Kullanıcı, 2fa kurtarma kodunu hatalı girdi.";
                log.LogStatusBadge = "badge bg-danger";
                log.LogStatus = "Uyarı";

                _db.Logs.Add(log);
                _db.SaveChanges();

                _logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
                ModelState.AddModelError(string.Empty, "Kurtarma kodu hatalı.");
                return Page();
            }
        }
    }
}
